using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerRoot _playerRoot;
    private Rigidbody _playerRigid;
    private Animator _animator;
    private PlayerInput _playerInput;
    public LayerMask groundLayer;
    public float playerHeight;
    public float moveSpeed = 1.0f;
    public float runSpeed = 3.0f;
    public float jumpForce = 5.0f;
    public float rotateSensitivity = 2.0f;

    void Start()
    {
        _playerRoot = GetComponent<PlayerRoot>();
        _playerRigid = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        if (_playerRoot.Model.isMine)
        {
            _playerInput.OnWalkInput += Walk;
            _playerInput.OnRunInput += Run;
            _playerInput.OnJumpInput += Jump;
            _playerInput.OnMouseInput += RotateBody;
            _playerInput.OnCrawlInput += Crawl;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Walk(Vector3 input)
    {
        if (input.sqrMagnitude < 0.1f)
        {
            _animator.SetFloat("moveSpeed", 0f);
            return;
        }

        Vector3 moveDirection = transform.rotation * input;
        Vector3 movement = moveDirection * moveSpeed;
        _playerRigid.MovePosition(_playerRigid.position + movement * Time.deltaTime);
        _animator.SetFloat("moveSpeed", 0.5f);
    }

    void Run(Vector3 input)
    {
        if (input.sqrMagnitude < 0.1f)
        {
            _animator.SetFloat("moveSpeed", 0f);
            return;
        }

        Vector3 moveDirection = transform.rotation * input;
        Vector3 movement = moveDirection * runSpeed;
        _playerRigid.MovePosition(_playerRigid.position + movement * Time.deltaTime);
        _animator.SetFloat("moveSpeed", 1f);
    }

    void Crawl()
    {
        _animator.SetBool("isCrawl", !_animator.GetBool("isCrawl"));
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
