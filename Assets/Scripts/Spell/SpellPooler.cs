using System.Collections.Generic;
using UnityEngine;

public class Spell
{
    public GameObject Object;
    public ISpell Type;
};

public class SpellPooler : MonoBehaviour
{

    [SerializeField] private int _spellAmountPerType;
    [SerializeField] private GameObject[] _spellsType;
    public static SpellPooler s_Instance;
    private Dictionary<SpellType, List<Spell>> _pooledSpells;

    private void Awake()
    {
        if (s_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        s_Instance = this;
    }

    private void Start()
    {
        _pooledSpells = new Dictionary<SpellType, List<Spell>>();
        for (int i = 0; i < _spellsType.Length; i++)
        {
            SpellType spellTypeName = _spellsType[i].GetComponent<ISpell>().Type;

            if (!_pooledSpells.ContainsKey(spellTypeName))
                _pooledSpells[spellTypeName] = new List<Spell>();

            for (int j = 0; j < _spellAmountPerType; j++)
            {
                GameObject spellObj = Instantiate(_spellsType[i]);
                spellObj.SetActive(false);
                spellObj.transform.SetParent(transform);

                ISpell spellType = spellObj.GetComponent<ISpell>();
                Spell spell = new()
                {
                    Object = spellObj,
                    Type = spellType,
                };
                _pooledSpells[spellTypeName].Add(spell);
            }
        }
    }

    public Spell GetPoolerSpell(SpellType spellType)
    {
        if (_pooledSpells.TryGetValue(spellType, out List<Spell> spells))
        {
            foreach (Spell spell in spells)
            {
                if (!spell.Object.activeInHierarchy)
                    return spell;
            }
        }
        return null;
    }
}
