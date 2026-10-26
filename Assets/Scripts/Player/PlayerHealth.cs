using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float _maxHealth;

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
