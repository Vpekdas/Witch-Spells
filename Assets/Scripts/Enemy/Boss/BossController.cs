using UnityEngine;

public enum BossState
{

}

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{

    [SerializeField] Rigidbody2D.SlideMovement _slideMovement;

    [SerializeField] float _horizontalSpeed;
    [SerializeField] float _jumpSpeed;

    [SerializeField] float _dashSpeed;
    [SerializeField] float _gravity;

    public int Phase;
    public Vector2 Velocity => _velocity;
    public Transform PlayerTransform;

    private bool _isGrounded;
    private bool _isJumping;

    private Rigidbody2D _rb2D;
    private Rigidbody2D.SlideResults _slideResults;
    private Vector2 _velocity;


    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _isGrounded = true;
        Phase = 1;
    }

    private void FixedUpdate()
    {
        if (!_isGrounded)
        {
            _velocity.y -= _gravity * Time.fixedDeltaTime;
        }

        Chase();

        _isGrounded = false;

        MoveAndSlide();
    }

    private void MoveAndSlide()
    {
        _slideResults = _rb2D.Slide(_velocity, Time.fixedDeltaTime, _slideMovement);
        if (_slideResults.slideHit)
        {
            Vector2 normal = _slideResults.slideHit.normal;
            // Bottom collision.
            if (normal.y > 0.7f)
            {
                _isGrounded = true;
                _velocity.y = 0.0f;
            }
        }
    }

    private void Idle()
    {
        _velocity = Vector2.zero;
    }

    private void Chase(bool withJumping = false)
    {
        Vector2 targetDir = PlayerTransform.position - gameObject.transform.position;
        _velocity.x = targetDir.normalized.x * _horizontalSpeed;
        if (_isGrounded && withJumping)
        {
            Jump();
        }
    }

    private void Jump()
    {
        _isJumping = true;
        _velocity.y = _jumpSpeed;
        _isGrounded = false;
    }


    private void Dash()
    {
        Vector2 targetDir = PlayerTransform.position - gameObject.transform.position;
        float directionX = targetDir.normalized.x;
        _velocity.x = _dashSpeed * directionX;
    }
}
