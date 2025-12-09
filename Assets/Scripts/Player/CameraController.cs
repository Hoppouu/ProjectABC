using UnityEngine;
using UnityEngine.LowLevel;

public class CameraController : MonoBehaviour
{
    private PlayerRoot _playerRoot;
    private PlayerInput _playerInput;
    private Vector2 _inputDelta;
    public float mouseSensitivity = 2f;
    public float smoothSpeed = 20f;
    private float _currentXRotation;

    private void Awake()
    {
        _playerRoot = GetComponentInParent<PlayerRoot>();
        _playerInput = GetComponentInParent<PlayerInput>();
    }

    private void Start()
    {
        if (_playerRoot.Model.isMine)
        {
            _playerInput.OnMouseInput += HandleLookInput;
        }
        else if (!_playerRoot.Model.isMine)
        {

        }
    }
    private void HandleLookInput(Vector2 input)
    {
        _inputDelta = input;
    }

    void LateUpdate()
    {
        if (_playerRoot == null || _playerRoot.headTransform == null) return;
        Vector3 stablePosition = _playerRoot.playerTransform.position;
        float bobbingHeight = _playerRoot.headTransform.position.y; 

        Vector3 targetWorldPos = new Vector3(stablePosition.x, bobbingHeight, stablePosition.z);

        // 카메라 위치 확정
        transform.position = targetWorldPos;


        // --- 회전 로직 (기존과 동일) ---
        if (_inputDelta.sqrMagnitude >= 0.001f)
        {
            float mouseX = _inputDelta.x * mouseSensitivity;
            float mouseY = _inputDelta.y * mouseSensitivity;

            _currentXRotation -= mouseY;
            _currentXRotation = Mathf.Clamp(_currentXRotation, -90f, 90f);

            // 몸통 회전
            _playerRoot.playerTransform.Rotate(Vector3.up * mouseX);
            _inputDelta = Vector2.zero;
        }

        Quaternion targetRotation = Quaternion.Euler(_currentXRotation, _playerRoot.playerTransform.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}