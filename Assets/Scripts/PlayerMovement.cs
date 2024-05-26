using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;

    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private BoxCollider2D _boxCollider;
    private float _wallJumpCoolDown;
    private float _horizontalInput;

    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int JumpTrigger = Animator.StringToHash("Jump");

    private bool IsGroundColliding =>
        Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0, Vector2.down, 0.01f, groundMask)
            .collider is not null;

    private bool IsWallColliding =>
        Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0,
                new Vector2(transform.localScale.x, 0), 0.01f, wallMask)
            .collider is not null;
    
    public bool CanAttack => _horizontalInput == 0 && IsGroundColliding && !IsWallColliding;

    private void Awake()
    {
        //Grab references for rigidbody and animator from object
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // print($"WallColliding {IsWallColliding}");
        // print($"GroundColliding {IsGroundColliding}");
        print($"_wallJumpCoolDown {_wallJumpCoolDown}");
        print($"delta time {Time.deltaTime}");

        _horizontalInput = Input.GetAxis("Horizontal");

        //Flip player when moving left-right
        if (_horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (_horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1f);

        //Set animator parameters
        _animator.SetBool(Run, _horizontalInput != 0f);
        _animator.SetBool(Grounded, IsGroundColliding);

        if (_wallJumpCoolDown > 0.2f)
        {
            _rigidBody.velocity = new Vector2(_horizontalInput * speed, _rigidBody.velocity.y);

            if (IsWallColliding && !IsGroundColliding)
            {
                _rigidBody.gravityScale = 0;
                _rigidBody.velocity = Vector2.zero;
            }
            else
                _rigidBody.gravityScale = 3;

            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
            _wallJumpCoolDown += Time.deltaTime;
    }

    private void Jump()
    {
        if (IsGroundColliding)
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, jumpPower);
            _animator.SetTrigger(JumpTrigger);
        }
        else if (IsWallColliding && !IsGroundColliding)
        {
            if (_horizontalInput == 0)
            {
                _rigidBody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y,
                    transform.localScale.z);
            }
            else
                _rigidBody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

            _wallJumpCoolDown = 0;
        }
    }
}