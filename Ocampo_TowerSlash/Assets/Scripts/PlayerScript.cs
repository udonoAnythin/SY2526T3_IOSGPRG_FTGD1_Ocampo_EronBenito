using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float _lives = 3;
    private Queue<EnemyScript> _enemies = new Queue<EnemyScript>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();

        if (enemy != null)
        {
            enemy.GetDetectedByPlayer();

            _enemies.Enqueue(enemy);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyScript>() == _enemies.Peek())
        {
            //Kill Enemy
            //EnemyScript enemy = _enemies.Dequeue();
            //enemy.TakeDamage();
            Debug.Log("Enemy Exited");
            _enemies.Dequeue();

            //Reduce Lives
            TakeDamage();

            //if all lives are lost, kill player

        }
    }

    public void AttackNearestEnemy(SwipeDirection playerSwipe)
    {
        if (_enemies.Count > 0)
        {
            //_enemies.Peek().AttackEnemy(playerSwipe);
            if (_enemies.Peek().Direction == playerSwipe)
            {
                
                EnemyScript enemy = _enemies.Dequeue();
                enemy.TakeDamage();
                GainLifeThroughChance();
                
            }
            else
            {
                EnemyScript enemy = _enemies.Dequeue();
                enemy.TakeDamage();

                TakeDamage();
            }
                
        }
    }

    private void TakeDamage()
    {
        _lives--;

        if ( _lives <= 0)
        {
            //Kill Player
        }
    }

    private void GainLifeThroughChance()
    {
        float chance = Random.Range(0f, 1f);
        if (chance <= 0.03f)
        {
            _lives++;
        }
    }
    
}
