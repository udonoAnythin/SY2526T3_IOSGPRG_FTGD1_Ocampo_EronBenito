using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnerScript : MonoBehaviour
{
    [Header("Grass Area Reference")]
    [SerializeField] private SpriteRenderer _grassSprite;

    [Header("Transform Parent")]
    [SerializeField] private GameObject _obstacleSpawnParent;

    [Header("Obstacle References")]
    [SerializeField] private List<GameObject> _obstaclePrefabs;

    [Header("Obstacle Variables")]
    [SerializeField] private List<int> _obstacleBatches;
    [SerializeField] private List<int> _obstaclesPerBatch;

    public void SpawnObstacles()
    {

        for(int i = 0; i < _obstaclePrefabs.Count; i++)
        {

            for (int j = 0; j < _obstacleBatches.Count; j++)
            {
                

                // Get Random Area
                Vector2 spawnArea = new Vector2(_grassSprite.size.x / 2, _grassSprite.size.y / 2);
                Vector2 spawnOrigin = Vector2.zero;

                // Detect if area near the spawn origin already has a collider
                // Loop until it doesnt
                Collider2D colliderWithinArea = null;
                do
                {
                    Vector2 obstacleHalfSize = _obstaclePrefabs[i].GetComponent<SpriteRenderer>().bounds.size;

                    spawnOrigin = new Vector2(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y));
                    colliderWithinArea = Physics2D.OverlapBox(spawnOrigin, obstacleHalfSize, 0);
                } while (colliderWithinArea != null);

                int clusterCount = Random.Range(1, _obstaclesPerBatch[i] + 1);
                for (int k = 0; k < clusterCount; k++)
                {
                    Vector2 spawnOffset = Vector2.zero;
                    int checkerLimit = 60;
                    do
                    {
                        Vector2 obstacleHalfSize = _obstaclePrefabs[i].GetComponent<SpriteRenderer>().bounds.size;

                        // Contain spawn within area
                        do
                        {
                            spawnOffset = new Vector2(Random.Range(-obstacleHalfSize.x, obstacleHalfSize.x), Random.Range(-obstacleHalfSize.y, obstacleHalfSize.y));
                        } while (
                                spawnOrigin.x + spawnOffset.x < -spawnArea.x ||
                                spawnOrigin.x + spawnOffset.x > spawnArea.x ||
                                spawnOrigin.y + spawnOffset.y < -spawnArea.y ||
                                spawnOrigin.y + spawnOffset.y > spawnArea.y);

                        colliderWithinArea = Physics2D.OverlapBox(spawnOrigin + spawnOffset, obstacleHalfSize, 0);
                        checkerLimit--;

                        if (checkerLimit == 0) spawnOffset = Vector2.zero;

                    } while (colliderWithinArea != null && checkerLimit > 0);

                    if (spawnOffset == Vector2.zero) continue;

                    // Instantiate on place
                    GameObject newBullet = Instantiate(_obstaclePrefabs[i], spawnOrigin + spawnOffset, Quaternion.identity);
                    newBullet.transform.SetParent(_obstacleSpawnParent.transform);


                }

            }

        }

        
    }
}
