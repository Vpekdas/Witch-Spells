using UnityEngine;

public interface IEnemy
{
    bool IsGrounded { get; }
    string Type { get; }
    float Damage { get; }
    float Speed { get; }
    float Gravity { get; set; }
    Vector2 Velocity { get; set; }
    Vector3 Position { get; set; }
    Vector3 Scale { get; set; }

    void Run();
}