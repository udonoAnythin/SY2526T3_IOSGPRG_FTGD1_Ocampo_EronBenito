using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStatsScript : EnemyStatsScript
{

    [SerializeField] private float _cameraDeathShakeIntensity = 0.3f;
    [SerializeField] private float _cameraDeathShakeTime = 0.3f;

    [Header("Loot Data")]
    [SerializeField] private GunScript _rocketLauncherData;
    [SerializeField] private AmmoScript _rocketGrenadeData;

    protected override void OnDeath(EntityStatsScript victim, EntityStatsScript killer)
    {
        CameraScript.instance.ShakeCamera(_cameraDeathShakeTime, _cameraDeathShakeIntensity);

        if (killer is PlayerStatsScript)
        {
            PlayerStatsScript player = killer as PlayerStatsScript;
            player.GetComponent<PlayerShootScript>().CollectGun(_rocketLauncherData);
            player.GetComponent<PlayerShootScript>().CollectBullets(_rocketGrenadeData);
        }

        Destroy(gameObject);
    }
}
