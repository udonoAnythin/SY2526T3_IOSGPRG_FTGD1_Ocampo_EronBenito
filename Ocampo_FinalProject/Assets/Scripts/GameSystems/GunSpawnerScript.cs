using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSpawnerScript : MonoBehaviour
{
    [Header("Grass Area Reference")]
    [SerializeField] private SpriteRenderer _grassSprite;

    [Header("Transform Parent")]
    [SerializeField] private GameObject _gunSpawnParent;

    [Header("Gun Prefabs")]
    [SerializeField] private GameObject _pistolPickupPrefab;
    [SerializeField] private GameObject _autoRiflePickupPrefab;
    [SerializeField] private GameObject _shotgunPickupPrefab;

    [Header("Spawn Chances")]
    [SerializeField] private float _weaponSpawnChance;

    [Header("Max Spawn Counts")]
    [SerializeField] private int _weaponSpawnCount;

    [Header("Boxcast Half Sizes")]
    [SerializeField] private float _weaponHalfSize;

    public void SpawnAllGuns()
    {
        SpawnGuns(_pistolPickupPrefab);
        SpawnGuns(_autoRiflePickupPrefab);
        SpawnGuns(_shotgunPickupPrefab);
    }

    private void SpawnGuns(GameObject gunPrefab)
    {
        for (int i = 0; i < _weaponSpawnCount; i++)
        {
            // Identify Chance of spawning
            float spawnCheck = Random.Range(0f, 1f);
            if (spawnCheck <= _weaponSpawnChance) continue;

            // Get Random Area
            Vector2 spawnArea = new Vector2(_grassSprite.size.x / 2, _grassSprite.size.y / 2);
            Vector2 spawnOrigin = Vector2.zero;

            // Detect if area nea the spawn origin already has a collider
            // Loop until it doesnt
            Collider2D colliderWithinArea = null;
            do
            {
                spawnOrigin = new Vector2(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y));
                colliderWithinArea = Physics2D.OverlapBox(spawnOrigin, new Vector2(_weaponHalfSize, _weaponHalfSize), 0);
            } while (colliderWithinArea != null);

            // Spawn Weapon
            GameObject newGun = Instantiate(gunPrefab, spawnOrigin, Quaternion.identity);
            newGun.transform.SetParent(_gunSpawnParent.transform);
        }
    }
}
