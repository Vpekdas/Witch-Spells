using System;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

[RequireComponent(typeof(SpellManager))]
public class PlayerController : MonoBehaviour
{
    private static readonly int s_IsJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int s_IsDashingHash = Animator.StringToHash("isDashing");
    private static readonly int s_VelocityHash = Animator.StringToHash("Velocity");

    [SerializeField] private float _horizontalSpeed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _wallJumpSpeed;
    [SerializeField] private float _wallJumpHeight;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] float _gravity;
    [SerializeField] float _wallJumpDuration;
    [SerializeField] private float _groundAccel;
    [SerializeField] private float _groundDeccel;
    [SerializeField] private float _airAccel;
    [SerializeField] private float _airDeccel;
    [SerializeField] Rigidbody2D.SlideMovement _slideMovement;
    [SerializeField] private GameObject _portal;
    private SpellManager _spellManager;



    private const float k_FallMultiplier = 2.5f;
    private const float k_CoyoteTime = 0.15f;
    private const float k_DashTime = 1.0f;

    private bool _isGrounded;
    private bool _isWalled;
    private bool _isWallJumping;
    private bool _isJumping;
    private bool _isDashing;
    private bool _isFacingRight;

    private bool _jumpRequested;
    private bool _jumpReleased;
    private bool _dashRequested;

    private float _coyoteTimeCounter;
    private float _dashTimeCounter;
    private float _wallJumpTimer;
    private float _horizontalInput;
    private Rigidbody2D _rb2D;
    private Rigidbody2D.SlideResults _slideResults;
    private Animator _animator;
    private Vector2 _velocity;
    private InputSystem_Actions _input;


    public bool IsFacingRight => _isFacingRight;
    public float MaxFallSpeed => _maxFallSpeed;
    public Vector2 Velocity => _velocity;



    private void Awake()
    {
        _input = new InputSystem_Actions();
        _input.Player.Enable();


        _rb2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _isGrounded = true;
        _isFacingRight = true;

        _input.Player.Spell1.performed += _ => OnSkillPressed(0);
        _input.Player.Spell2.performed += _ => OnSkillPressed(1);
        _input.Player.Spell3.performed += _ => OnSkillPressed(2);
        _input.Player.Spell4.performed += _ => OnSkillPressed(3);

        _spellManager = GetComponent<SpellManager>();
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
            _animator.SetBool(s_IsJumpingHash, _isJumping);
            _coyoteTimeCounter = k_CoyoteTime;
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
            if (_dashRequested && _dashTimeCounter > k_DashTime)
            {
                Dash();
            }
        }

        // Short jump.
        if (_jumpReleased && _velocity.y > 0)
        {
            _velocity.y = 0.0f;
        }

        if (_dashRequested && _dashTimeCounter > k_DashTime)
        {
            Dash();
        }
        else
        {
            _dashRequested = false;
            _isDashing = false;
            _animator.SetBool(s_IsDashingHash, _isDashing);
        }

        if (!_isGrounded && !_isWalled && !_isWallJumping)
        {
            _velocity.y -= _gravity * Time.fixedDeltaTime;
        }

        // Increase speed when falling.
        if (_velocity.y < 0)
        {
            _velocity.y -= _gravity * k_FallMultiplier * Time.fixedDeltaTime;
            _velocity.y = Math.Max(_velocity.y, -_maxFallSpeed);
        }


        _animator.SetFloat(s_VelocityHash, Math.Abs(_velocity.x));


        _isGrounded = false;
        _isWalled = false;

        MoveAndSlide();
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
    }

    private void OnSkillPressed(int slot)
    {
        CastSpell(_spellManager.GetSpell(slot));
    }

    private void MoveAndSlide()
    {
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
        _animator.SetBool(s_IsJumpingHash, _isJumping);
        _velocity.y = _jumpSpeed;
        _isGrounded = false;
        _jumpRequested = false;
        _isWalled = false;
    }

    private void WallJump(float normal)
    {
        _isWallJumping = true;
        _animator.SetBool(s_IsJumpingHash, _isWallJumping);
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
        _animator.SetBool(s_IsDashingHash, _isDashing);
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

    private void CastSpell(SpellId id)
    {
        if (id == SpellId.None)
        {
            return;
        }

        GameObject spellObj = SpellPooler.s_Instance.GetPoolerSpell(id);
        if (spellObj == null)
        {
            return;
        }


        if (id != SpellId.Shield)
        {
            _portal.SetActive(true);
        }

        spellObj.SetActive(true);
        ISpell spell = spellObj.GetComponent<ISpell>();
        Vector2 dir = _isFacingRight ? Vector2.right : Vector2.left;

        if (spellObj.TryGetComponent(out SpriteRenderer sr))
        {
            sr.flipX = !_isFacingRight;
        }

        spell.Cast(_portal.transform.position, dir);
    }
}
