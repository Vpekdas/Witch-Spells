using UnityEngine;

public interface IEnemy
{
    bool IsGrounded { get; set; }
    string Type { get; set; }
    float Damage { get; set; }
    float Health { get; set; }
    float Speed { get; set; }
    float Gravity { get; set; }
    Vector2 Velocity { get; set; }
    Vector3 Position { get; set; }
    Vector3 Scale { get; set; }

    void Run();
}