using System.Collections.Generic;
using UnityEngine;



public class SpellPooler : MonoBehaviour
{

    [SerializeField] private int _spellAmountPerType;
    [SerializeField] private GameObject[] _spellsType;
    public static SpellPooler s_Instance;
    private Dictionary<SpellId, List<GameObject>> _pooledSpells;
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
        _pooledSpells = new();

        foreach (GameObject prefab in _spellsType)
        {
            ISpell spellComponent = prefab.GetComponent<ISpell>();
            SpellId id = spellComponent.Id;

            if (!_pooledSpells.ContainsKey(id))
            {
                _pooledSpells[id] = new List<GameObject>();
            }

            for (int i = 0; i < _spellAmountPerType; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                _pooledSpells[id].Add(obj);
            }
        }
    }

    public GameObject GetPoolerSpell(SpellId id)
    {
        if (!_pooledSpells.TryGetValue(id, out List<GameObject> list))
        {
            return null;
        }

        foreach (GameObject obj in list)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        return null;
    }
}
