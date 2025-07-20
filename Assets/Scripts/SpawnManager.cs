using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private EnemyPooler _enemyPooler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Enemy enemy = _enemyPooler.GetPoolerEnemy("Green Slime");
        if (enemy != null)
        {
            enemy.Object.SetActive(true);
            enemy.Type.Position = Vector3.zero;
        }
    }
}
