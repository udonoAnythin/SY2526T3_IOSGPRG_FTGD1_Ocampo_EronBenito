using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    Pistol = 0,
    AutomaticRifle = 1,
    Shotgun = 2
}

public class GunScript : MonoBehaviour
{

    public GunData GunData
    { get => _gunData; }

    [SerializeField] private GunData _gunData;
    [SerializeField] private int _minLoadedBullets;
    [SerializeField] private int _maxLoadedBullets;

    private void Awake()
    {
        SpawnGun();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerGunScript player = collision.GetComponent<PlayerGunScript>();

        if (player != null)
        {
            player.CollectGun(this);
            Destroy(gameObject);
        }
    }

    private void SpawnGun()
    {
        // Instantiate a copy of the Gun Data
        _gunData = Instantiate(_gunData);

        _gunData.currentLoadedBullets = Random.Range(_minLoadedBullets, _maxLoadedBullets);
    }

}
