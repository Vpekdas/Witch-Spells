using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private EnemyPooler _enemyPooler;
    [SerializeField] private GameObject[] _spawnPointsObj;

    // Register all spawn point events
    private void Awake()
    {
        for (int i = 0; i < _spawnPointsObj.Length; i++)
        {
            SpawnPoint spawnPoint = _spawnPointsObj[i].GetComponent<SpawnPoint>();
            spawnPoint.OnReadyToSpawn += SpawnEnemy;
        }
    }

    private void SpawnEnemy(SpawnPoint spawnPoint)
    {
        Enemy enemy = _enemyPooler.GetPoolerEnemy("Green Slime");
        if (enemy != null)
        {
            enemy.Object.transform.position = spawnPoint.transform.position;
            enemy.Object.SetActive(true);
        }
    }
}
