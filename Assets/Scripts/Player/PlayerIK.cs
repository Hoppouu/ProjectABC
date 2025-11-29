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

        if (lookTarget == null)
        {
            lookTarget = GetComponent<Camera>().transform;
        }
    }
    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (lookTarget != null)
        {
            animator.SetLookAtWeight(lookWeight, 0.3f, 0.8f, 1.0f, 0.5f);
            Vector3 lookPos = lookTarget.position + lookTarget.forward * 10f;
            animator.SetLookAtPosition(lookPos);
        }
    }
}