using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerModel playerModel;
    public Vector3 viewOffset;
    public float mouseSensitivity = 2f;

    private float _mouseX;
    private float _mouseY;
    private float _currentXRotation;

    void OnEnable()
    {
        if (GameManager.inst != null)
            GameManager.inst.inputHandler.OnMouseInput += HandleInput;
    }

    void OnDisable()
    {
        if (GameManager.inst != null)
            GameManager.inst.inputHandler.OnMouseInput -= HandleInput;
    }

    void HandleInput(Vector2 input)
    {
        _mouseX = input.x * mouseSensitivity;
        _mouseY = input.y * mouseSensitivity;
    }

    // [1] 몸통 회전은 Update (애니메이션보다 먼저)
    void Update()
    {
        if (playerModel.transform != null && Mathf.Abs(_mouseX) > 0.001f)
        {
            playerModel.transform.Rotate(Vector3.up * _mouseX);
        }
    }

    void LateUpdate()
    {
        if (playerModel.transform == null || playerModel.headPos == null) return;

        // --- 회전 (즉시 반응 유지) ---
        _currentXRotation -= _mouseY;
        _currentXRotation = Mathf.Clamp(_currentXRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(_currentXRotation, playerModel.transform.eulerAngles.y, 0f);

        // --- 위치 (노이즈 필터링 적용) ---
        Vector3 rootPos = playerModel.transform.position;
        float rawHeadHeight = playerModel.headPos.position.y; // 덜덜 떨리는 좀비 머리 높이

        // [핵심 해결책] 현재 카메라 높이와 목표 높이(머리) 사이를 부드럽게 보간
        // Time.deltaTime * 15f 정도면 꿀렁임은 따라가되, 미세한 떨림은 무시합니다.
        float smoothedHeight = Mathf.Lerp(transform.position.y, rawHeadHeight, 15f * Time.deltaTime);

        // X, Z는 몸통(안정적), Y는 부드럽게 처리된 높이 사용
        Vector3 finalPos = new Vector3(rootPos.x, smoothedHeight, rootPos.z);

        // 오프셋 적용
        transform.position = finalPos + (transform.rotation * viewOffset);

        _mouseX = 0; _mouseY = 0;
    }
}