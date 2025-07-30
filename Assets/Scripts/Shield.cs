using UnityEngine;

public class Shield : MonoBehaviour, ISpell
{
    [SerializeField] private float _damage, _speed;
    [SerializeField] private string _type;
    private Vector2 _direction;

    public float Damage { get => _damage; set => _damage = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public string Type { get => _type; set => _type = value; }
    public Vector2 Direction { get => _direction; set => _direction = value; }
    public Vector3 Position { get => transform.position; set => transform.position = value; }
    public Vector3 Scale { get => transform.localScale; set => transform.localScale = value; }

    public void OnShieldAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
