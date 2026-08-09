using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncherScript : GunScript
{
    protected override void Awake()
    {
        SpawnGun();
    }

    protected override void SpawnGun()
    {
        // Instantiate a copy of the Gun Data
        _gunData = Instantiate(_gunData);

        _gunData.currentLoadedBullets = Random.Range(_minLoadedBullets, _maxLoadedBullets);
    }
}
