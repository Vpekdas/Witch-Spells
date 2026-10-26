using UnityEngine;

public enum SpellId
{
    None,
    FireBall,
    Shield

}

public interface ISpell
{
    SpellId Id { get; }
    float Damage { get; }
    float Speed { get; }

    void Cast(Vector3 position, Vector2 direction);
    void ResetSpell();
}
