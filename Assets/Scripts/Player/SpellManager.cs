using UnityEngine;

public class SpellManager : MonoBehaviour
{

    private readonly SpellId[] _equippedSpells = new SpellId[4];

    private void Awake()
    {
        _equippedSpells[0] = SpellId.FireBall;
        _equippedSpells[1] = SpellId.Shield;
    }

    public SpellId GetSpell(int slot)
    {
        return _equippedSpells[slot];
    }

    public void SetSpell(int slot, SpellId id)
    {
        _equippedSpells[slot] = id;
    }

}
