using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawnerScript : MonoBehaviour
{
    [Header("Grass Area Reference")]
    [SerializeField] private SpriteRenderer _grassSprite;

    [Header("Transform Parent")]
    [SerializeField] private GameObject _ammoSpawnParent;

    [Header("Ammo Prefabs")]
    [SerializeField] private GameObject _9mmPrefab;
    [SerializeField] private GameObject _556mmPrefab;
    [SerializeField] private GameObject _12gPrefab;


    [Header("Spawn Chances")]
    [SerializeField] private float _ammoSpawnChance;

    [Header("Max Spawn Counts")]
    [SerializeField] private int _ammoSpawnCount;
    [SerializeField] private int _ammoClusterCount;

    [Header("Boxcast Half Sizes")]
    [SerializeField] private float _ammoHalfSize;

    public void SpawnAllBullets()
    {
        SpawnBullets(_9mmPrefab);
        SpawnBullets(_556mmPrefab);
        SpawnBullets(_12gPrefab);
    }


    private void SpawnBullets(GameObject ammoPrefab)
    {
        for (int i = 0; i < _ammoSpawnCount; i++)
        {
            // Identify Chance of spawning
            float spawnChanceCheck = Random.Range(0f, 1f);
            if (spawnChanceCheck <= _ammoSpawnChance) continue;

            // Get Random Area
            Vector2 spawnArea = new Vector2(_grassSprite.size.x / 2, _grassSprite.size.y / 2);
            Vector2 spawnOrigin = Vector2.zero;

            // Detect if area near the spawn origin already has a collider
            // Loop until it doesnt
            Collider2D colliderWithinArea = null;
            do
            {
                spawnOrigin = new Vector2(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y));
                colliderWithinArea = Physics2D.OverlapBox(spawnOrigin, new Vector2(_ammoHalfSize, _ammoHalfSize), 0);
            } while (colliderWithinArea != null);

            int clusterCount = Random.Range(1, _ammoClusterCount + 1);
            for (int j = 0; j < clusterCount; j++)
            {
                Vector2 spawnOffset = Vector2.zero;
                int checkerLimit = 10;
                do
                {
                    spawnOffset = new Vector2(Random.Range(-_ammoHalfSize * 2, _ammoHalfSize * 2), Random.Range(-_ammoHalfSize * 2, _ammoHalfSize * 2));
                    colliderWithinArea = Physics2D.OverlapBox(spawnOrigin + spawnOffset, new Vector2(_ammoHalfSize, _ammoHalfSize), 0);
                    checkerLimit--;

                    if (checkerLimit == 0) spawnOffset = Vector2.zero;

                } while (colliderWithinArea != null && checkerLimit > 0);

                if (spawnOffset == Vector2.zero) continue;
                
                // Instantiate on place
                GameObject newBullet = Instantiate(ammoPrefab, spawnOrigin + spawnOffset, Quaternion.identity);
                newBullet.transform.SetParent(_ammoSpawnParent.transform);
            }

        }
    }

    


}
