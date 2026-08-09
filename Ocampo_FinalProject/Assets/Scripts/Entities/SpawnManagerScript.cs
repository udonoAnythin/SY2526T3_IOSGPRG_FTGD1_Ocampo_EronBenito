using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerScript : MonoBehaviour
{
    [SerializeField] private ObstacleSpawnerScript _obstacleSpawner;
    [SerializeField] private GunSpawnerScript _gunSpawner;
    [SerializeField] private AmmoSpawnerScript _ammoSpawner;
    [SerializeField] private EnemySpawnerScript _enemySpawner;

    private void Start()
    {
        if (_obstacleSpawner != null)
            _obstacleSpawner.SpawnObstacles();

        if (_gunSpawner != null)
            _gunSpawner.SpawnAllGuns();
        
        if (_ammoSpawner != null)
            _ammoSpawner.SpawnAllBullets();

        if (_enemySpawner != null)
            _enemySpawner.SpawnAllEnemies();
    }

}
