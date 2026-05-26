using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    private List<EnemyScript> _enemies = new List<EnemyScript>();

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _spawnLocation;
    [SerializeField] private float _spawnMinInterval;
    [SerializeField] private float _spawnMaxInterval;

    [Header("Speed Up Values")]
    [SerializeField] private bool _areEnemiesSpeedingUp = false;

    private void Start()
    {
        StartCoroutine(TimerScript.Instance.CO_ExecuteInRandomIntervals(SpawnEnemy, _spawnMinInterval, _spawnMaxInterval));
    }

    public void SpeedUpEnemies()
    {

    }

    public void RevertEnemies()
    {

    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(_enemyPrefab, _spawnLocation.transform.position, Quaternion.identity);
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();

        enemyScript.Initialize();
        _enemies.Add(enemyScript);
    }



}
