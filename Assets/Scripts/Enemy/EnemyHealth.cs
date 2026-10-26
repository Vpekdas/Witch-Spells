using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] private float _maxHealth = 10f;
    public float Health => _health;
    private float _health;


    private void Awake()
    {
        _health = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
            Die();
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}
