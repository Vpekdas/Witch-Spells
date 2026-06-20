using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private static readonly int s_VelocityHash = Animator.StringToHash("Velocity");

    [SerializeField] float _horizontalSpeed, _jumpSpeed, _wallJumpSpeed, _wallJumpHeight, _dashSpeed, _maxFallSpeed;
    [SerializeField] float _gravity;
    [SerializeField] float _wallJumpDuration;
    [SerializeField] float _groundAccel, _groundDeccel, _airAccel, _airDeccel;
    [SerializeField] float _health;
    [SerializeField] Rigidbody2D.SlideMovement _slideMovement;
    [SerializeField] private GameObject _portal;


    private readonly float _fallMultiplier = 2.5f;
    private readonly float _coyoteTime = 0.15f;
    private readonly float _dashTime = 1.0f;

    private bool _isGrounded, _isWalled, _isWallJumping, _isJumping, _isDashing, _isFacingRight;
    private bool _jumpRequested, _jumpReleased, _dashRequested;
    private float _coyoteTimeCounter, _dashTimeCounter, _wallJumpTimer;
    private float _horizontalInput;
    private Rigidbody2D _rb2D;
    private Rigidbody2D.SlideResults _slideResults;
    private Animator _animator;
    private Vector2 _velocity;

    public bool IsFacingRight => _isFacingRight;
    public float MaxFallSpeed => _maxFallSpeed;
    public Vector2 Velocity => _velocity;
    public SpellPooler SpellPooler;
    private InputSystem_Actions _input;


    private void Awake()
    {
        _input = new InputSystem_Actions();
        _input.Player.Enable();


        _rb2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _isGrounded = true;
        _isWalled = false;
        _isWallJumping = false;
        _isFacingRight = true;
        _isJumping = false;
        _isDashing = false;

        _jumpRequested = false;
        _jumpReleased = false;
        _dashRequested = false;

        _coyoteTimeCounter = 0.0f;
        _dashTimeCounter = 0.0f;
        _wallJumpTimer = 0.0f;

        _velocity = new Vector2();
    }


    private void Update()
    {
        _horizontalInput = _input.Player.Move.ReadValue<Vector2>().x;

        if (_horizontalInput != 0)
        {
            bool wantsRight = _horizontalInput > 0;

            if (_isFacingRight != wantsRight)
            {
                Turn();
            }
        }

        PlayerInput();
    }

    private void PlayerInput()
    {
        if (_input.Player.Jump.WasPressedThisFrame())
        {
            _jumpRequested = true;
            _jumpReleased = false;
        }
        if (_input.Player.Jump.WasReleasedThisFrame())
        {
            _jumpReleased = true;
        }
        if (_input.Player.Dash.WasPressedThisFrame())
        {
            _dashRequested = true;
        }
        if (_input.Player.FirstSpell.WasPressedThisFrame())
        {
            CastSpell(SpellType.FireBall);
        }
        if (_input.Player.Shield.WasPressedThisFrame())
        {
            CastSpell(SpellType.Shield);
        }
    }

    private void FixedUpdate()
    {
        if (Math.Abs(_velocity.x) <= 0.01f)
        {
            _velocity.x = 0.0f;
        }

        _coyoteTimeCounter -= Time.fixedDeltaTime;

        if (_isGrounded)
        {
            _isJumping = false;
            _animator.SetBool("isJumping", _isJumping);
            _coyoteTimeCounter = _coyoteTime;
            _slideMovement.gravity = new Vector2(0.0f, -3.0f);
            _isWalled = false;
            _dashTimeCounter += Time.fixedDeltaTime;
        }

        if (_isWallJumping)
        {
            _wallJumpTimer -= Time.fixedDeltaTime;
            if (_wallJumpTimer < 0f)
            {
                _isWallJumping = false;
            }
        }

        if (!_isWalled)
        {
            Run(1.0f);
        }


        if (_jumpRequested && _coyoteTimeCounter > 0.0f)
        {
            Jump();
            if (_dashRequested && _dashTimeCounter > _dashTime)
            {
                Dash();
            }
        }

        // Short jump.
        if (_jumpReleased && _velocity.y > 0)
        {
            _velocity.y = 0.0f;
        }

        if (_dashRequested && _dashTimeCounter > _dashTime)
        {
            Dash();
        }
        else
        {
            _dashRequested = false;
            _isDashing = false;
            _animator.SetBool("isDashing", _isDashing);
        }

        if (!_isGrounded && !_isWalled && !_isWallJumping)
        {
            _velocity.y -= _gravity * Time.fixedDeltaTime;
        }

        // Increase speed when falling.
        if (_velocity.y < 0)
        {
            _velocity.y -= _gravity * _fallMultiplier * Time.fixedDeltaTime;
            _velocity.y = Math.Max(_velocity.y, -_maxFallSpeed);
        }


        _animator.SetFloat(s_VelocityHash, Math.Abs(_velocity.x));


        _isGrounded = false;
        _isWalled = false;

        _slideResults = _rb2D.Slide(_velocity, Time.fixedDeltaTime, _slideMovement);

        // https://www.gamedev.net/forums/topic/632771-how-can-my-program-detect-if-the-player-hits-a-wallfloor-ceiling/
        if (_slideResults.slideHit)
        {
            Vector2 normal = _slideResults.slideHit.normal;

            // If collision is too slope, don't climb.
            float angle = Vector2.Angle(normal, Vector2.up);
            if (angle > _slideMovement.surfaceSlideAngle)
            {
                _velocity.x = 0f;
            }

            // Top collision.
            if (normal.y < -0.7f)
            {
                _velocity.y = 0.0f;
                _isWalled = false;
            }
            // Bottom collision.
            if (normal.y > 0.7f)
            {
                _isGrounded = true;
                _velocity.y = 0.0f;
            }
            // Left / Right collision.
            if ((normal.x > 0.7 || normal.x < -0.7) && !_isGrounded)
            {
                _slideMovement.gravity = new Vector2(0.0f, -1.0f);
                _velocity.y = 0.0f;
                _isWalled = true;

                if (_jumpRequested && !_isGrounded)
                {
                    WallJump(normal.x);
                }
            }

        }
    }
    private void Jump()
    {
        _isJumping = true;
        _animator.SetBool("isJumping", _isJumping);
        _velocity.y = _jumpSpeed;
        _isGrounded = false;
        _jumpRequested = false;
        _isWalled = false;
    }

    private void WallJump(float normal)
    {
        _isWallJumping = true;
        _animator.SetBool("isJumping", _isWallJumping);
        _velocity.x = _wallJumpSpeed * Math.Clamp(normal, -1.0f, 1.0f);
        _velocity.y = _wallJumpHeight;
        _isGrounded = false;
        _jumpRequested = false;
        _isWalled = false;
        _wallJumpTimer = _wallJumpDuration;
    }

    private void Run(float lerpAmount)
    {
        float targetSpeed = _horizontalInput * _horizontalSpeed;
        targetSpeed = Mathf.Lerp(_velocity.x, targetSpeed, lerpAmount);
        float accelRate = _isGrounded
            ? (Mathf.Abs(targetSpeed) > 0.01f ? _groundAccel : _groundDeccel)
            : (Mathf.Abs(targetSpeed) > 0.01f ? _airAccel : _airDeccel);
        float speedDiff = targetSpeed - _velocity.x;
        float movement = speedDiff * accelRate * Time.fixedDeltaTime;
        _velocity.x += movement;
    }

    private void Dash()
    {
        _isDashing = true;
        _animator.SetBool("isDashing", _isDashing);
        _dashTimeCounter = 0.0f;
        float direction = _isFacingRight ? 1.0f : -1.0f;
        _velocity.x = _dashSpeed * direction;
        _dashRequested = false;
    }


    private void Turn()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void CastSpell(SpellType spellType)
    {
        Spell spell = SpellPooler.s_Instance.GetPoolerSpell(spellType);
        if (spell != null)
        {
            if (spell.Type.Type != SpellType.Shield)
            {
                _portal.SetActive(true);
            }
            spell.Object.SetActive(true);
            spell.Type.Direction = _isFacingRight ? Vector2.right : Vector2.left;
            Vector3 scale = spell.Type.Scale;
            scale.x = _isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            spell.Type.Scale = scale;
            spell.Type.Position = _portal.transform.position;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.CompareTag("Enemy"))
        {
            _health -= obj.GetComponent<IEnemy>().Damage;
            if (_health <= 0.0f)
            {
                Debug.Log("You died !");
            }
        }
    }


}
