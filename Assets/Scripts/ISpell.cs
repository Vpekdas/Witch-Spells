using UnityEngine;

public interface ISpell
{
    string Type { get; }
    float Damage { get; }
    Vector3 Position { get; set; }
    Vector2 Direction { get; set; }
    float Speed { get; }
    Vector3 Scale { get; set; }
}
