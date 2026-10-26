using System.Collections.Generic;
using UnityEngine;

public class EnemyPooler : MonoBehaviour
{
    public static EnemyPooler s_Instance;
    [SerializeField] private int _enemyAmount;
    [SerializeField] private GameObject[] _enemyPrefabs;
    private Dictionary<EnemyId, List<GameObject>> _pooledEnemy;


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
        _pooledEnemy = new();

        foreach (GameObject prefab in _enemyPrefabs)
        {
            EnemyController enemyComponent = prefab.GetComponent<EnemyController>();
            EnemyId id = enemyComponent.Id;

            if (!_pooledEnemy.ContainsKey(id))
            {
                _pooledEnemy[id] = new List<GameObject>();
            }

            for (int i = 0; i < _enemyAmount; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                _pooledEnemy[id].Add(obj);
            }
        }
    }

    public GameObject GetPoolerEnemy(EnemyId id)
    {
        if (!_pooledEnemy.TryGetValue(id, out List<GameObject> list))
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


