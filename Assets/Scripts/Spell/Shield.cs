using UnityEngine;

public class Shield : MonoBehaviour, ISpell
{
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;

    [SerializeField] private SpellId _id;
    public SpellId Id { get => _id; set => _id = value; }
    public float Damage { get => _damage; set => _damage = value; }
    public float Speed { get => _speed; set => _speed = value; }

    public void OnShieldAnimationEnd()
    {
        gameObject.SetActive(false);
    }

    public void ResetSpell()
    {
    }

    public void Cast(Vector3 position, Vector2 direction)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

}
