using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _spawnLocation;
    [SerializeField] private float _spawnInterval;

    private List<EnemyScript> _enemies = new List<EnemyScript>();

    

    private void Start()
    {
        StartCoroutine(TimerScript.Instance.CO_ExecuteInSecondIntervals(SpawnEnemy, _spawnInterval));
    }

    private void Update()
    {
        RemoveNullEnemiesFromList();
    }

    private void SpawnEnemy()
    {
        
        GameObject enemy = Instantiate(_enemyPrefab, _spawnLocation.transform.position, Quaternion.identity);
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();

        enemyScript.Initialize();
        _enemies.Add(enemyScript);
    }

    private void RemoveNullEnemiesFromList()
    {
        foreach (EnemyScript enemy in _enemies)
        {
            if (enemy == null)
            {
                RemoveEnemyFromList(enemy);
                break;
            }
        }
    }

    private void RemoveEnemyFromList(EnemyScript enemy)
    {
        _enemies.Remove(enemy);
    }


}
