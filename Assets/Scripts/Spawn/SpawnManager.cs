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
        GameObject enemyObj = _enemyPooler.GetPoolerEnemy(EnemyId.Slime);
        if (enemyObj == null)
        {
            return;
        }
        enemyObj.transform.position = spawnPoint.transform.position;
        enemyObj.SetActive(true);
    }
}
