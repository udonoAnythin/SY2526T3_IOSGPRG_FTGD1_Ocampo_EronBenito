using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Queue<EnemyScript> _enemies = new Queue<EnemyScript>();

    public void KillNearestEnemy()
    {
        EnemyScript enemy = _enemies.Dequeue();
        SpawnerScript.Instance.RemoveEnemyFromList(enemy);
        enemy.KillEnemy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();

        Debug.Log("TRIGGERED");

        if (enemy != null)
        {
            enemy.GetDetectedByPlayer();

            _enemies.Enqueue(enemy);
        }
    }

    
}
