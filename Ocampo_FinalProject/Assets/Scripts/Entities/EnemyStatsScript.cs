using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsScript : EntityStatsScript
{

    protected override void Awake()
    {
        base.Awake();

        _onDeath.AddListener(OnDeath);
    }

    private void OnDeath()
    {
        Debug.Log("Enemy Died!");
        Destroy(gameObject);
    }


}
