using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoScript : MonoBehaviour
{
    public GunType GunType
    { get => _gunType; }

    public int AmmoCount
    { get => _ammoCount; }

    [SerializeField] private GunType _gunType;
    [SerializeField] private int _ammoCount;

    [SerializeField] private int _maxAmmo;
    [SerializeField] private int _minAmmo;

    private void Awake()
    {
        _ammoCount = Random.Range(_minAmmo, _maxAmmo);
    }
}
