using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _playerRigid;
    private Animator _animator;
    private PlayerModel _playerModel;
    public LayerMask groundLayer;
    public float playerHeight;
    public float moveSpeed = 1.0f;
    public float runSpeed = 3.0f;
    public float jumpForce = 5.0f;
    public float rotateSensitivity = 2.0f;
    void OnEnable()
    {
        _playerRigid = GetComponent<Rigidbody>();
        _playerModel = GetComponent<PlayerModel>();
        _animator = GetComponent<Animator>();
        _playerModel.inputHandler = GetComponent<InputHandler>();
        _playerModel.inputHandler.OnWalkInput += Walk;
        _playerModel.inputHandler.OnRunInput += Run;
        _playerModel.inputHandler.OnJumpInput += Jump;
        _playerModel.inputHandler.OnMouseInput += RotateBody;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
    }

    void Walk(Vector3 input)
    {
        if (input.sqrMagnitude < 0.1f)
        {
            _animator.SetBool("isWalk", false);
            _animator.SetBool("isRun", false);
            return;
        }

        Vector3 moveDirection = transform.rotation * input;
        Vector3 movement = moveDirection * moveSpeed;
        _playerRigid.MovePosition(_playerRigid.position + movement * Time.deltaTime);
        _animator.SetBool("isWalk", true);
        _animator.SetBool("isRun", false);
    }

    void Run(Vector3 input)
    {
        if (input.sqrMagnitude < 0.1f)
        {
            _animator.SetBool("isRun", false);
            _animator.SetBool("isWalk", false);
            return;
        }

        Vector3 moveDirection = transform.rotation * input;
        Vector3 movement = moveDirection * runSpeed;
        _playerRigid.MovePosition(_playerRigid.position + movement * Time.deltaTime);
        _animator.SetBool("isWalk", false);
        _animator.SetBool("isRun", true);
    }

    void RotateBody(Vector2 mouseInput)
    {
        float yRotation = mouseInput.x * rotateSensitivity;
        transform.Rotate(Vector3.up * yRotation);
    }

    void Jump()
    {
        if (IsGrounded())
        {
            _playerRigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight, groundLayer);
    }

    void Update()
    {
        
    }
}
