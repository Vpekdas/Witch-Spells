using UnityEngine;

public class GreenSlime : MonoBehaviour, IEnemy
{
    [SerializeField] private string _type;
    [SerializeField] private float _damage, _speed, _gravity;
    [SerializeField] Rigidbody2D.SlideMovement _slideMovement;

    private bool _isGrounded, _isFacingRight;
    private Vector2 _velocity;
    private Rigidbody2D _rb2D;
    private Rigidbody2D.SlideResults _slideResults;

    public string Type { get => _type; set => _type = value; }
    public float Damage { get => _damage; set => _damage = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public float Gravity { get => _gravity; set => _gravity = value; }
    public bool IsGrounded { get => _isGrounded; set => _isGrounded = value; }
    public Vector2 Velocity { get => _velocity; set => _velocity = value; }
    public Vector3 Position { get => transform.position; set => transform.position = value; }
    public Vector3 Scale { get => transform.localScale; set => transform.localScale = value; }

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _velocity = new Vector2();
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
        _slideResults = _rb2D.Slide(_velocity, Time.fixedDeltaTime, _slideMovement);

        if (_slideResults.surfaceHit)
        {
            _isGrounded = true;
            _velocity.y = 0.0f;
        }
        else
        {
            _isGrounded = false;
        }

        if (_slideResults.slideHit)
        {
            Vector2 normal = _slideResults.slideHit.normal;
            // Top collision
            if (normal.y < -0.7f)
            {
            }
            // Left / Right collision
            if (normal.x > 0.7)
            {
                _isFacingRight = true;
            }
            else if (normal.x < -0.7)
            {
                _isFacingRight = false;
            }
            _velocity.x = 0.0f;
            _velocity.y = 0.0f;
        }
    }

    public void Run()
    {
        float direction = _isFacingRight ? 1.0f : -1.0f;
        float movement = direction * _speed * Time.fixedDeltaTime;
        _velocity.x += movement;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spell"))
        {
            gameObject.SetActive(false);
        }
    }
}
