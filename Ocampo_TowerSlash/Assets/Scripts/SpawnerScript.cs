using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _spawnLocation;
    [SerializeField] private float _spawnInterval;

    private void Start()
    {
        StartCoroutine(TimerScript.Instance.CO_ExecuteInSecondIntervals(SpawnEnemy, _spawnInterval));
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(_enemyPrefab, _spawnLocation.transform.position, Quaternion.identity);
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();

        enemyScript.Initialize();
    }

}
