using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RocketBulletScript : BulletScript
{

    [SerializeField] private float _rocketBlastRange;
    [SerializeField] private float _rocketBlastDamageMultiplier;
    [SerializeField] private GameObject _rocketBlastParticles;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        // Stops checking if it collides with other bullets
        if (collision.gameObject.GetComponent<BulletScript>() != null) return;

        // Damages entities
        EntityStatsScript entity = collision.gameObject.GetComponent<EntityStatsScript>();
        if (entity != null)
        {
            Debug.Log(entity);
            entity.TakeDamage(Damage, owner);

            // Spawn Entity Particles
            GameObject newParticles = Instantiate(_entityHitParticles, transform.position, Quaternion.identity);
            GameObject.Destroy(newParticles, 1);
        }
        else
        {
            // Spawn Obstacle Particles
            GameObject newParticles = Instantiate(_obstacleHitParticles, transform.position, Quaternion.identity);
            GameObject.Destroy(newParticles, 1);
        }

        // Take account of the blast
        Collider2D[] collaterals;
        collaterals = Physics2D.OverlapCircleAll(transform.position, _rocketBlastRange);

        foreach (Collider2D collateral in collaterals)
        {
            if (collateral.gameObject.GetComponent<EntityStatsScript>() != null)
            {
                EntityStatsScript affectedEntity = collateral.gameObject.GetComponent<EntityStatsScript>();
                if (affectedEntity == owner) continue;

                float aoeDamagePercent = Vector3.Distance(affectedEntity.transform.position, transform.position) / _rocketBlastRange;
                int finalDamage = (int)(Damage * aoeDamagePercent * _rocketBlastDamageMultiplier);
                affectedEntity.TakeDamage(finalDamage, owner);
            }
        }

        // Spawn Obstacle Particles
        GameObject blastParticles = Instantiate(_rocketBlastParticles, transform.position, Quaternion.identity);
        GameObject.Destroy(blastParticles, 1.5f);

        Destroy(gameObject);
    }

}
