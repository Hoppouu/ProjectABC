using UnityEngine;
using UnityEngine.Rendering;

public class PlayerSight : MonoBehaviour
{
    public Animator animator;
    public Transform lookTarget;

    private PlayerRoot _playerRoot;
    private PlayerInput _playerInput;
    private SkinnedMeshRenderer[] _skinnedMeshRenderer;
    [SerializeField] private Light _flashLight;

    void Start()
    {
        animator = GetComponent<Animator>();
        lookTarget = GetComponentInChildren<Camera>().transform;
        _playerRoot = GetComponent<PlayerRoot>();
        _playerInput = GetComponent<PlayerInput>();
        if (!_playerRoot.Model.isMine)
        {
            _skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in _skinnedMeshRenderer)
            {
                renderer.shadowCastingMode = ShadowCastingMode.On;
            }
        }
        else
        {
            _playerInput.OnLightInput += LightOnOff;
        }
    }

    void LightOnOff()
    {
        if (_flashLight.gameObject.activeSelf)
        {
            _flashLight.gameObject.SetActive(false);
        }
        else if (!_flashLight.gameObject.activeSelf)
        {
            _flashLight.gameObject.SetActive(true);
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (lookTarget != null)
        {
            // 1. 얼마나 강하게 쳐다볼지 설정 (가중치)
            // SetLookAtWeight(전체강도, 몸통, 머리, 눈, 제한강도)
            animator.SetLookAtWeight(1f, 0.3f, 0.8f, 1.0f, 0.5f);

            // 2. 어디를 쳐다볼지 계산
            // 타겟(카메라)의 위치에서 앞쪽으로 10만큼 떨어진 지점을 계산합니다.
            Vector3 lookPos = lookTarget.position + lookTarget.forward * 10f;

            // 3. 시선 적용
            animator.SetLookAtPosition(lookPos);
        }
    }

    void Update()
    {
        
    }
}
