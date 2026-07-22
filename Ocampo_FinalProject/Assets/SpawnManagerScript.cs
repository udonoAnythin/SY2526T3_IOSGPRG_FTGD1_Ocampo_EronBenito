using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerScript : MonoBehaviour
{

    [SerializeField] private GunSpawnerScript _gunSpawner;
    [SerializeField] private AmmoSpawnerScript _ammoSpawner;

    private void Start()
    {
        _gunSpawner.SpawnAllGuns();
        _ammoSpawner.SpawnAllBullets();
    }

}
