using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsScript : EntityStatsScript
{

    protected override void Awake()
    {
        base.Awake();

        onDeath.AddListener(OnDeath);
    }

    protected virtual void OnDeath(EntityStatsScript victim, EntityStatsScript killer)
    {
        Destroy(gameObject);
    }


}
