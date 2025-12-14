using UnityEngine;
using Network;

public class PlayerController : MonoBehaviour
{
    private PlayerRoot _playerRoot;
    private Rigidbody _playerRigid;
    private Animator _animator;
    private PlayerInput _playerInput;

    private bool _isCrawl = false;
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
        if (_playerRoot.Model.IsMine)
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

    void Update()
    {
        TriggerAnimation();
    }

    void TriggerAnimation()
    {
        SetPlayerPostureState(_playerRoot.Model.PlayerPostureState);
        SetPlayerMovementState(_playerRoot.Model.PlayerMovementType);
        _animator.SetBool("isCrawl", _isCrawl);
        _animator.SetFloat("moveSpeed", moveSpeed);
    }

    public void SetPlayerPostureState(PlayerPostureState postureState)
    {
        switch (postureState)
        {
            case PlayerPostureState.Stand:
                _isCrawl = false;   break;
            case PlayerPostureState.Crawl:
                _isCrawl = true;    break;
        }
        _playerRoot.Model.SetPlayerState(postureState);
    }

    public void SetPlayerMovementState(PlayerMovementType movementType)
    {
        switch (movementType)
        {
            case PlayerMovementType.Idle:
                moveSpeed = 0f;     break;
            case PlayerMovementType.Walk:
                moveSpeed = 0.5f;   break;
            case PlayerMovementType.Run:
                moveSpeed = 1f;     break;
        }
        _playerRoot.Model.SetPlayerState(movementType);
    }

    void Walk(Vector3 input)
    {
        if (input.sqrMagnitude < 0.1f)
        {
            SetPlayerMovementState(PlayerMovementType.Idle);
            return;
        }

        Vector3 moveDirection = transform.rotation * input;
        Vector3 movement = moveDirection * moveSpeed;
        _playerRigid.MovePosition(_playerRigid.position + movement * Time.deltaTime);
        SetPlayerMovementState(PlayerMovementType.Walk);
    }

    void Run(Vector3 input)
    {
        if (input.sqrMagnitude < 0.1f)
        {
            SetPlayerMovementState(PlayerMovementType.Idle);
            return;
        }

        Vector3 moveDirection = transform.rotation * input;
        Vector3 movement = moveDirection * runSpeed;
        _playerRigid.MovePosition(_playerRigid.position + movement * Time.deltaTime);
        SetPlayerMovementState(PlayerMovementType.Run);
    }

    void Crawl()
    {
        if (_animator.GetBool("isCrawl"))
        {
            SetPlayerPostureState(PlayerPostureState.Stand);
        }
        else
        {
            SetPlayerPostureState(PlayerPostureState.Crawl);
        }
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
}
