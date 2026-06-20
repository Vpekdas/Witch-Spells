using UnityEngine;

public enum SpellType
{
    FireBall,
    Shield

}

public interface ISpell
{
    SpellType Type { get; set; }
    float Damage { get; set; }
    Vector3 Position { get; set; }
    Vector2 Direction { get; set; }
    float Speed { get; set; }
    Vector3 Scale { get; set; }
}
