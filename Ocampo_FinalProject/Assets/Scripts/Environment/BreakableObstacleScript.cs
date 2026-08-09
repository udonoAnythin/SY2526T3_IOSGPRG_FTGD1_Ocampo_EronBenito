using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObstacleScript : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int _health = 3;
    [SerializeField] private float _scaleDownValue = 0.6f;

    [Header("Ammo Spawn Values")]
    [SerializeField] private List<GameObject> _ammoPrefabs = new List<GameObject>();
    [SerializeField] private float _bulletSpawnOffset;
    [SerializeField] private int _maxBulletSpawnCount;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BulletScript>() != null)
        {
            if (_health > 0)
            {
                _health--;
                transform.localScale *= _scaleDownValue;
            }
            else
            {
                SpawnBullets();
                Destroy(gameObject);
            }

        }
    }

    private void SpawnBullets()
    {
        GameObject bulletToInstantiate = _ammoPrefabs[Random.Range(0, _ammoPrefabs.Count)];
        float bulletSpawnCount = Random.Range(1, _maxBulletSpawnCount + 1);

        for (int i = 0; i < bulletSpawnCount; i++)
        {
            Vector2 offset = Quaternion.Euler(0f, 0f, Random.Range(0, 360)) * Vector2.right * _bulletSpawnOffset;
            Instantiate(bulletToInstantiate, transform.position + new Vector3(offset.x, offset.y, 0), Quaternion.identity);
        }

    }

}
