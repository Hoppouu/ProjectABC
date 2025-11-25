using UnityEngine;

public class PlayerIK : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform lookTarget;
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    [Header("Settings")]
    [Range(0, 1)] public float lookWeight = 1f; // 1이면 완전히 쳐다봄
    [Range(0, 1)] public float handWeight = 1f; // 1이면 손을 완전히 고정함

    void Start()
    {
        // Animator 자동 할당
        if (animator == null) animator = GetComponent<Animator>();

        // lookTarget이 없으면 카메라를 기준으로 임시 설정
        if (lookTarget == null)
        {
            lookTarget = Camera.main.transform;
        }
    }

    // 유니티 애니메이션 시스템이 뼈대를 계산할 때 호출되는 특수 함수
    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // 1. 머리/목 회전 (Look At)
        if (lookTarget != null)
        {
            // 시선 가중치 설정 (전체, 몸통, 머리, 눈, 0~1 사이 값)
            // 몸통(0.3)은 조금만, 머리(0.8)는 많이 돌리게 설정
            animator.SetLookAtWeight(lookWeight, 0.3f, 0.8f, 1.0f, 0.5f);

            // 실제로 쳐다볼 위치: 카메라 위치 + 카메라가 보는 방향 * 멀리(10m)
            Vector3 lookPos = lookTarget.position + lookTarget.forward * 10f;
            animator.SetLookAtPosition(lookPos);
        }

        if (rightHandTarget != null)
        {
            // 위치와 회전 가중치 설정
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handWeight);

            // 손을 target 위치로 강제 이동
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
        }
        if (leftHandTarget != null)
        {
            // 위치와 회전 가중치 설정
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handWeight);

            // 손을 target 위치로 강제 이동
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
    }
}