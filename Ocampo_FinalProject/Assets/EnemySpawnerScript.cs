using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    [Header("Grass Area Reference")]
    [SerializeField] private SpriteRenderer _grassSprite;

    [Header("Transform Parent")]
    [SerializeField] private GameObject _enemySpawnParent;

    [Header("Gun Data")]
    [SerializeField] private List<GunData> _guns;

    [Header("Enemy Prefab")]
    [SerializeField] private GameObject _enemyPrefab;

    [Header("Spawn Counts")]
    [SerializeField] private int _enemySpawnCount;

    [Header("Collision Half Sizes")]
    [SerializeField] private float _enemyHalfSize;

    public void SpawnAllEnemies()
    {
        for (int i = 0; i < _enemySpawnCount; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        // Get Random Area
        Vector2 spawnArea = new Vector2(_grassSprite.size.x / 2, _grassSprite.size.y / 2);
        Vector2 spawnOrigin = Vector2.zero;

        // Detect if area nea the spawn origin already has a collider
        // Loop until it doesnt
        Collider2D colliderWithinArea = null;
        do
        {
            spawnOrigin = new Vector2(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y));
            colliderWithinArea = Physics2D.OverlapBox(spawnOrigin, new Vector2(_enemyHalfSize, _enemyHalfSize), 0);
        } while (colliderWithinArea != null);

        // Spawn enemy
        GameObject newEnemy = Instantiate(_enemyPrefab, spawnOrigin, Quaternion.identity);
        newEnemy.transform.SetParent(_enemySpawnParent.transform);

        // Spawn Weapon
        GunData newGun = Instantiate(_guns[Random.Range(0, _guns.Count)], newEnemy.transform);
        newEnemy.GetComponent<EnemyFSMScript>().InitializeGun(newGun);
        
    }
}
