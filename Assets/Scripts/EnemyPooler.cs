using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public GameObject Object;
    public IEnemy Type;
};

public class EnemyPooler : MonoBehaviour
{
    [SerializeField] private int _enemyAmountPerType;
    [SerializeField] private GameObject[] _enemyType;
    public static EnemyPooler s_Instance;
    private Dictionary<string, List<Enemy>> _pooledEnemy;

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
        _pooledEnemy = new Dictionary<string, List<Enemy>>();
        for (int i = 0; i < _enemyType.Length; i++)
        {
            string enemyTypeName = _enemyType[i].GetComponent<IEnemy>().Type;

            if (!_pooledEnemy.ContainsKey(enemyTypeName))
                _pooledEnemy[enemyTypeName] = new List<Enemy>();

            for (int j = 0; j < _enemyAmountPerType; j++)
            {
                GameObject enemyObj = Instantiate(_enemyType[i]);
                enemyObj.SetActive(false);
                enemyObj.transform.SetParent(transform);

                IEnemy enemyType = enemyObj.GetComponent<IEnemy>();
                Enemy enemy = new()
                {
                    Object = enemyObj,
                    Type = enemyType,
                };
                _pooledEnemy[enemyTypeName].Add(enemy);
            }
        }
    }

    public Enemy GetPoolerEnemy(string enemyType)
    {
        if (_pooledEnemy.TryGetValue(enemyType, out List<Enemy> enemies))
        {
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.Object.activeInHierarchy)
                    return enemy;
            }
        }
        return null;
    }
}
