using System;
using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private float _spawnTimer;
    private bool _timerOn;
    private float _timer;
    private int _enemyCount;
    private Coroutine _runningCoroutine;
    public event Action<SpawnPoint> OnReadyToSpawn;

    private void Awake()
    {
        _timer = _spawnTimer;
        _timerOn = false;
        _enemyCount = 0;
    }

    private void Start()
    {
        AreaIsClear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            _enemyCount++;
            if (_timerOn)
            {
                _timerOn = false;
                if (_runningCoroutine != null && gameObject.activeInHierarchy)
                {
                    StopCoroutine(_runningCoroutine);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _enemyCount = Mathf.Max(0, _enemyCount - 1);
            if (_enemyCount == 0)
            {
                _timer = _spawnTimer;
                if (!_timerOn && gameObject.activeInHierarchy)
                {
                    _timerOn = true;
                    _runningCoroutine = StartCoroutine(SpawnTimerRoutine());
                }
            }
        }
    }

    private IEnumerator SpawnTimerRoutine()
    {
        while (_timer > 0.0f)
        {
            _timer -= Time.deltaTime;
            yield return null;
        }
        _timerOn = false;
        AreaIsClear();
    }

    // Spawn request only if no enemy was detected in given spawn time.
    private void AreaIsClear()
    {
        OnReadyToSpawn?.Invoke(this);
    }
}