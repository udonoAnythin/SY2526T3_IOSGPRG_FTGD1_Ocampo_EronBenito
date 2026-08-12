using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{

    public GunData GunData
    { get => _gunData; }

    [SerializeField] protected GunData _gunData;
    [SerializeField] protected int _minLoadedBullets;
    [SerializeField] protected int _maxLoadedBullets;

    protected virtual void Awake()
    {
        SpawnGun();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerShootScript player = collision.GetComponent<PlayerShootScript>();

        if (player != null)
        {
            player.CollectGun(this);
            Destroy(gameObject);
        }
    }

    protected virtual void SpawnGun()
    {
        // Instantiate a copy of the Gun Data
        _gunData = Instantiate(_gunData);

        _gunData.currentLoadedBullets = Random.Range(_minLoadedBullets, _maxLoadedBullets);
    }

}
