using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Queue<EnemyScript> _enemies = new Queue<EnemyScript>();

    public void AttackNearestEnemy(SwipeDirection playerSwipe)
    {
        if (_enemies.Count > 0)
        {
            _enemies.Peek().AttackEnemy(playerSwipe);
        }
    }

    private void Update()
    {
        DetectIfEnemiesAreKilled();
    }

    private void DetectIfEnemiesAreKilled()
    {
        if (_enemies.Count <= 0) return;
            
        if (_enemies.Peek() == null)
        {
            RemoveEnemyFromQueue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();

        //Debug.Log("TRIGGERED");

        if (enemy != null)
        {
            enemy.GetDetectedByPlayer();

            _enemies.Enqueue(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyScript>() == _enemies.Peek())
        {
            RemoveEnemyFromQueue();
        }
    }

    private void RemoveEnemyFromQueue()
    {
        EnemyScript enemy = _enemies.Dequeue();
    }
    
}
