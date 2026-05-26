using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _spawnLocation;
    [SerializeField] private float _spawnMinInterval;
    [SerializeField] private float _spawnMaxInterval;

    private List<EnemyScript> _enemies = new List<EnemyScript>();
    private float _currentSpeedMultiplier = 1;
    private Coroutine _spawnerCoroutine;

    private void Start()
    {
        StartSpawningEnemies();
    }

    public void StartSpawningEnemies()
    {
        _spawnerCoroutine = StartCoroutine(TimerScript.Instance.CO_ExecuteInRandomIntervals(SpawnEnemy, _spawnMinInterval, _spawnMaxInterval));
    }

    public void StopSpawningEnemies()
    {
        StopCoroutine(_spawnerCoroutine);
    }

    public void ChangeEnemySpeed(float multiplier)
    {
        _currentSpeedMultiplier = multiplier;

        foreach (EnemyScript enemy in _enemies)
        {
            enemy.ChangeSpeed(_currentSpeedMultiplier);
        }
    }

    public void RevertEnemySpeed()
    {
        _currentSpeedMultiplier = 1;

        foreach (EnemyScript enemy in _enemies)
        {
            enemy.RevertSpeed();
        }
    }

    public void RemoveEnemy(EnemyScript enemy)
    {
        _enemies.Remove(enemy);
    }

    public void ResetAllEnemies()
    {
        foreach(EnemyScript enemy in _enemies)
        {
            Destroy(enemy.gameObject);
        }

        _enemies.Clear();
    }

    public int CheckEnemyCount()
    {
        return _enemies.Count;
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(_enemyPrefab, _spawnLocation.transform.position, Quaternion.identity);
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();

        enemyScript.Initialize(this, _currentSpeedMultiplier);
        _enemies.Add(enemyScript);
    }



}
