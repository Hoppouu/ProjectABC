using UnityEngine;
using Player.Model;

public class PlayerController : MonoBehaviour
{
    #region 설정
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _playerHeight;
    [SerializeField] private float _runSpeed = 3.0f;
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _rotateSensitivity = 2.0f;
    #endregion

    #region private 변수
    private PlayerRoot _playerRoot;
    private Rigidbody _playerRigid;
    private Animator _animator;
    private PlayerInput _playerInput;
    #endregion

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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            _playerRoot.OnPlayerInfoUpdate += () => ApplyModelDataToView();
        }

    }
    private void Update()
    {
        if (_playerRoot.Model.isMine)
        {
            _playerRoot.Model.SetPlayerPosition(transform.position);
        }
    }
    private void ApplyModelDataToView()
    {
        _animator.SetBool("isCrawl", _playerRoot.Model.IsCrawl);
        _animator.SetFloat("moveSpeed", _playerRoot.Model.MoveSpeed);
        transform.position = _playerRoot.Model.Data.playerPosition;
        transform.rotation = Quaternion.Euler(_playerRoot.Model.Data.playerRotation);
    }

    void Walk(Vector3 input)
    {
        if (input.sqrMagnitude < 0.1f)
        {
            _playerRoot.Model.SetMovementType(PlayerMovementType.Idle);
            _animator.SetFloat("moveSpeed", _playerRoot.Model.MoveSpeed);
            return;
        }

        Vector3 moveDirection = transform.rotation * input;
        Vector3 movement = moveDirection * _playerRoot.Model.MoveSpeed;
        _playerRigid.MovePosition(_playerRigid.position + movement * Time.deltaTime);
        _playerRoot.Model.SetMovementType(PlayerMovementType.Walk);
        _animator.SetFloat("moveSpeed", _playerRoot.Model.MoveSpeed);
    }

    void Run(Vector3 input)
    {
        if (input.sqrMagnitude < 0.1f)
        {
            _playerRoot.Model.SetMovementType(PlayerMovementType.Idle);
            _animator.SetFloat("moveSpeed", _playerRoot.Model.MoveSpeed);
            return;
        }

        Vector3 moveDirection = transform.rotation * input;
        Vector3 movement = moveDirection * _runSpeed;
        _playerRigid.MovePosition(_playerRigid.position + movement * Time.deltaTime);
        _playerRoot.Model.SetMovementType(PlayerMovementType.Run);
        _animator.SetFloat("moveSpeed", _playerRoot.Model.MoveSpeed);
    }

    void Crawl()
    {
        if (_animator.GetBool("isCrawl"))
        {
            _playerRoot.Model.SetPostureState(PlayerPostureState.Stand);
        }
        else
        {
            _playerRoot.Model.SetPostureState(PlayerPostureState.Crawl);
        }
        _animator.SetBool("isCrawl", _playerRoot.Model.IsCrawl);
    }

    void RotateBody(Vector2 mouseInput)
    {
        float yRotation = mouseInput.x * _rotateSensitivity;
        transform.Rotate(Vector3.up * yRotation);
    }

    void Jump()
    {
        if (IsGrounded())
        {
            _playerRigid.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight, _groundLayer);
    }
}
