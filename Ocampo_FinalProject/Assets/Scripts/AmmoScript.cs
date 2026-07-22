using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoScript : MonoBehaviour
{
    public GunType GunType
    { get => _gunType; }

    public int AmmoCount
    { get => Random.Range(_minAmmo, _maxAmmo); }

    [SerializeField] private GunType _gunType;
    [SerializeField] private int _ammoCount;

    [SerializeField] private int _maxAmmo;
    [SerializeField] private int _minAmmo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerGunScript player = collision.GetComponent<PlayerGunScript>();

        if (player != null)
        {
            player.CollectBullets(this);
            Destroy(gameObject);
        }
    }
}
