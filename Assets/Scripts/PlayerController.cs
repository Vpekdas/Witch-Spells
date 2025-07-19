using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _horizontalSpeed, _jumpSpeed, _wallJumpSpeed, _wallJumpHeight, _dashSpeed, _maxFallSpeed;
    [SerializeField] float _gravity;
    [SerializeField] float _wallJumpDuration;
    [SerializeField] float _groundAccel, _groundDeccel, _airAccel, _airDeccel;
    [SerializeField] Rigidbody2D.SlideMovement _slideMovement;
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
    private InputAction _dash;
    private InputAction _firstSpell;
    private InputAction _shieldSpell;


    private Vector2 _velocity;

    public bool IsFacingRight => _isFacingRight;
    public float MaxFallSpeed => _maxFallSpeed;
    public Vector2 Velocity => _velocity;
    public SpellPooler SpellPooler;


    private void Awake()
    {
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

    private void Start()
    {
        // New input system.
        _dash = InputSystem.actions.FindAction("Dash");
        _firstSpell = InputSystem.actions.FindAction("First Spell");
        _shieldSpell = InputSystem.actions.FindAction("Shield");
    }

    // TODO: Use the new Input system
    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        if (_horizontalInput != 0)
        {
            if (_isFacingRight != IsMovingRight())
            {
                Turn();
            }
        }
        PlayerInput();
    }

    private void PlayerInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpRequested = true;
            _jumpReleased = false;
        }
        if (Input.GetButtonUp("Jump"))
        {
            _jumpReleased = true;
        }
        if (_dash.WasCompletedThisFrame())
        {
            _dashRequested = true;
        }
        if (_firstSpell.WasCompletedThisFrame())
        {
            CastSpell("FireBall");
        }
        if (_shieldSpell.WasCompletedThisFrame())
        {
            CastSpell("Shield");
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

        // Wall jumping
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

        // Handle jump with Coyote Timer
        if (_jumpRequested && _coyoteTimeCounter > 0.0f)
        {
            Jump();
            if (_dashRequested && _dashTimeCounter > _dashTime)
            {
                Dash();
            }
        }

        // Short jump
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

        // Apply gravity
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

        _animator.SetFloat("Velocity", Math.Abs(_velocity.x));
        _slideResults = _rb2D.Slide(_velocity, Time.fixedDeltaTime, _slideMovement);

        // Handle collision to surface (bottom)
        if (_slideResults.surfaceHit)
        {
            _isGrounded = true;
            _velocity.y = 0.0f;
        }
        else
        {
            _isGrounded = false;
        }

        // https://www.gamedev.net/forums/topic/632771-how-can-my-program-detect-if-the-player-hits-a-wallfloor-ceiling/
        if (_slideResults.slideHit)
        {
            Vector2 normal = _slideResults.slideHit.normal;
            // Top collision
            if (normal.y < -0.7f)
            {
                _velocity.y = 0.0f;
                _isWalled = false;
            }
            // Left / Right collision
            if ((normal.x > 0.7 || normal.x < -0.7) && !_isGrounded)
            {
                _slideMovement.gravity = new Vector2(0.0f, -1.0f);
                _velocity.y = 0.0f;
                _isWalled = true;
                if (_jumpRequested)
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
        float direction = IsMovingRight() ? 1.0f : -1.0f;
        _velocity.x = _dashSpeed * direction;
        _dashRequested = false;
    }

    private bool IsMovingRight()
    {
        return _velocity.x > 0.0f;
    }

    private void Turn()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void CastSpell(string spellType)
    {
        Spell spell = SpellPooler.s_Instance.GetPoolerSpell(spellType);
        if (spell != null)
        {
            spell.Object.SetActive(true);
            spell.Type.Direction = _isFacingRight ? Vector2.right : Vector2.left;
            Vector3 scale = spell.Type.Scale;
            scale.x = _isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            spell.Type.Scale = scale;
            spell.Type.Position = transform.position;
        }
        else
        {
            Debug.Log(spellType = " was not found");
        }
    }
}
