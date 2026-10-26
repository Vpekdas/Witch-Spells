using UnityEngine;

public class FireBall : MonoBehaviour, ISpell
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private SpellId _id = SpellId.FireBall;

    private Vector2 _direction;

    public SpellId Id => _id;
    public float Damage => _damage;
    public float Speed => _speed;

    public void Cast(Vector3 position, Vector2 direction)
    {
        transform.position = position;
        _direction = direction;
    }

    public void ResetSpell()
    {
        _direction = Vector2.zero;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.Translate(_speed * Time.deltaTime * _direction);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(Damage);
            ResetSpell();
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            ResetSpell();
        }

    }

}