using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    public float Health => _health;
    private float _health;

    private BossController _bossController;

    private void Awake()
    {
        _health = _maxHealth;
    }
    public void TakeDamage(float damage)
    {
        _health -= damage;

        UpdatePhase();

        if (_health <= 0)
            Die();
    }


    private void UpdatePhase()
    {
        if (_health <= _maxHealth * 0.5f)
            _bossController.Phase = 2;

    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}