using UnityEngine;
using UnityEngine.LowLevel;

public class CameraController : MonoBehaviour
{
    public PlayerModel playerModel;
    public Vector3 viewOffset;
    private Vector2 _inputDelta;
    public float mouseSensitivity = 2f;
    public float smoothSpeed = 20f;
    private float _currentXRotation;

    private void OnEnable()
    {
        playerModel.inputHandler.OnMouseInput += HandleLookInput;
    }
    private void HandleLookInput(Vector2 input)
    {
        _inputDelta = input;
    }

    void LateUpdate()
    {
        if (playerModel == null || playerModel.headPos == null) return;
        Vector3 stablePosition = playerModel.transform.position;
        float bobbingHeight = playerModel.headPos.position.y; 

        // 최종 카메라 위치 조합
        Vector3 targetWorldPos = new Vector3(stablePosition.x, bobbingHeight, stablePosition.z);

        // 오프셋 적용 (회전 방향 고려)
        // 주의: headBone.rotation을 쓰면 머리 돌릴 때 카메라도 돌아가므로 playerRoot의 회전을 기준 잡음
        Vector3 finalPos = targetWorldPos + (playerModel.transform.rotation * viewOffset);

        // 카메라 위치 확정
        transform.position = finalPos;


        // --- 회전 로직 (기존과 동일) ---
        if (_inputDelta.sqrMagnitude >= 0.001f)
        {
            float mouseX = _inputDelta.x * mouseSensitivity;
            float mouseY = _inputDelta.y * mouseSensitivity;

            _currentXRotation -= mouseY;
            _currentXRotation = Mathf.Clamp(_currentXRotation, -90f, 90f);

            // 몸통 회전
            playerModel.transform.Rotate(Vector3.up * mouseX);
            _inputDelta = Vector2.zero;
        }

        Quaternion targetRotation = Quaternion.Euler(_currentXRotation, playerModel.transform.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}