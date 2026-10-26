using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    private const float k_KNOCKBACK_STRENGTH = 5.0f;
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;

    [SerializeField] private float _gravity;

    [SerializeField] Rigidbody2D.SlideMovement _slideMovement;
    [SerializeField] private EnemyId _id;

    private bool _isGrounded;
    private bool _isFacingRight;
    private Vector2 _velocity;
    private Rigidbody2D _rb2D;
    private Rigidbody2D.SlideResults _slideResults;

    public EnemyId Id { get => _id; set => _id = value; }
    public float Damage => _damage;
    public Vector2 Velocity { get => _velocity; set => _velocity = value; }
    public Vector3 Position { get => transform.position; set => transform.position = value; }
    public Vector3 Scale { get => transform.localScale; set => transform.localScale = value; }

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _isGrounded = true;
        _isFacingRight = true;
    }

    private void FixedUpdate()
    {
        Run();

        if (!_isGrounded)
        {
            _velocity.y -= _gravity * Time.fixedDeltaTime;
        }

        MoveAndSlide();
    }

    private void MoveAndSlide()
    {
        _slideResults = _rb2D.Slide(_velocity, Time.fixedDeltaTime, _slideMovement);

        if (_slideResults.slideHit)
        {
            Vector2 normal = _slideResults.slideHit.normal;

            // Top collision.
            if (normal.y < -0.7f)
            {
                _velocity.y = 0.0f;
            }

            // Bottom collision.
            if (normal.y > 0.7f)
            {
                _isGrounded = true;
            }

            // Left / Right collision
            if (normal.x > 0.7)
            {
                _isFacingRight = true;
            }

            if (normal.x < -0.7)
            {
                _isFacingRight = false;
            }
        }
    }
    public void Run()
    {
        float direction = _isFacingRight ? 1.0f : -1.0f;
        _velocity.x = direction * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spell"))
        {
            ISpell spell = collision.GetComponent<ISpell>();

            if (spell.Id == SpellId.Shield)
            {
                float direction = _isFacingRight ? -1.0f : 1.0f;
                _velocity.x = k_KNOCKBACK_STRENGTH * direction;
            }

            spell.ResetSpell();

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(Damage);
        }
    }

    private void OnEnable()
    {
        _velocity = Vector2.zero;
        _isGrounded = true;
        _isFacingRight = true;
    }
}
