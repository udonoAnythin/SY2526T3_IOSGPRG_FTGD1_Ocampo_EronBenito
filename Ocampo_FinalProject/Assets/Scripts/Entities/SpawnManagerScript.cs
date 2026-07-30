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
        _obstacleSpawner.SpawnObstacles();
        _gunSpawner.SpawnAllGuns();
        _ammoSpawner.SpawnAllBullets();
        _enemySpawner.SpawnAllEnemies();
    }

}
