using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandGeneratorScript : MonoBehaviour
{

    [SerializeField] private SpriteRenderer _grass;
    [SerializeField] private GameObject _sandPrefab;
    [SerializeField] private float _sizeVariation = 2;
    [SerializeField] private float _sandIncrement = 10;

    private void Start()
    {
        SpawnSand();
    }

    private void SpawnSand()
    {
        Vector2 grassBounds = _grass.bounds.extents;

        // Top and bottom edges
        for (float x = _grass.bounds.min.x; x < _grass.bounds.max.x; x += _sandIncrement)
        {
            Vector2 sandCoords1 = new Vector2(x, _grass.bounds.max.y);
            GameObject sand1 = Instantiate(_sandPrefab, sandCoords1, Quaternion.Euler(0, 0, Random.Range(0, 359)));
            sand1.transform.localScale *= Random.Range(_sizeVariation - _sizeVariation / 2, _sizeVariation + _sizeVariation / 2);
            sand1.transform.SetParent(transform);

            Vector2 sandCoords2 = new Vector2(x, _grass.bounds.min.y);
            GameObject sand2 = Instantiate(_sandPrefab, sandCoords2, Quaternion.Euler(0, 0, Random.Range(0, 359)));
            sand2.transform.localScale *= Random.Range(_sizeVariation - _sizeVariation / 2, _sizeVariation + _sizeVariation / 2);
            sand2.transform.SetParent(transform);
        }

        // left and right edges
        for (float y = _grass.bounds.min.y; y < _grass.bounds.max.y; y += _sandIncrement)
        {
            Vector2 sandCoords1 = new Vector2(_grass.bounds.max.x, y);
            GameObject sand1 = Instantiate(_sandPrefab, sandCoords1, Quaternion.Euler(0, 0, Random.Range(0, 359)));
            sand1.transform.localScale *= Random.Range(_sizeVariation - _sizeVariation / 2, _sizeVariation + _sizeVariation / 2);
            sand1.transform.SetParent(transform);

            Vector2 sandCoords2 = new Vector2(_grass.bounds.min.x, y);
            GameObject sand2 = Instantiate(_sandPrefab, sandCoords2, Quaternion.Euler(0, 0, Random.Range(0, 359)));
            sand2.transform.localScale *= Random.Range(_sizeVariation - _sizeVariation / 2, _sizeVariation + _sizeVariation / 2);
            sand2.transform.SetParent(transform);
        }

    }

}
