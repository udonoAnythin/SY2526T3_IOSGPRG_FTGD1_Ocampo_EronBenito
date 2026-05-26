using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    public float Lives
    {
        get => _lives;
    }

    public bool IsDashing
    {
        get => _isDashing;
    }

    public UnityEvent PlayerDamaged
    {
        get => _playerDamage;
    }

    [SerializeField] private float _lives = 3;
    [SerializeField] private SwipeDetectionScript _swipeDetectionScript;

    [Header("Events")]
    [SerializeField] private UnityEvent _playerDamage = new UnityEvent();

    private Queue<EnemyScript> _enemies = new Queue<EnemyScript>();
    private bool _isDashing = false;

    private void OnEnable()
    {
        _swipeDetectionScript.SwipedLeft.AddListener(OnSwipeLeft);
        _swipeDetectionScript.SwipedRight.AddListener(OnSwipeRight);
        _swipeDetectionScript.SwipedDown.AddListener(OnSwipeDown);
        _swipeDetectionScript.SwipedUp.AddListener(OnSwipeUp);
    }

    private void OnDisable()
    {
        _swipeDetectionScript.SwipedLeft.RemoveListener(OnSwipeLeft);
        _swipeDetectionScript.SwipedRight.RemoveListener(OnSwipeRight);
        _swipeDetectionScript.SwipedDown.RemoveListener(OnSwipeDown);
        _swipeDetectionScript.SwipedUp.RemoveListener(OnSwipeUp);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();

        if (enemy != null)
        {
            if (_isDashing)
            {
                enemy.TakeDamage();
            }
            else
            {
                enemy.GetDetectedByPlayer();

                _enemies.Enqueue(enemy);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyScript>() == _enemies.Peek())
        {
            //Kill Enemy
            EnemyScript enemy = _enemies.Dequeue();
            enemy.TakeDamage();

            //Reduce Lives
            TakeDamage();

        }
    }

    public void SetDash(bool isDashing)
    {
        _isDashing = isDashing;

        while (_enemies.Count > 0)
        {
            EnemyScript enemy = _enemies.Dequeue();
            enemy.TakeDamage();
        }
    }

    private void AttackNearestEnemy(SwipeDirection playerSwipe)
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
                TakeDamage();
            }

        }
    }

    private void TakeDamage()
    {
        _lives--;

        //Invoke the event
        _playerDamage.Invoke();

        //Clear all in queue for reset
        _enemies.Clear();

    }

    private void GainLifeThroughChance()
    {
        float chance = Random.Range(0f, 1f);
        if (chance <= 0.03f)
        {
            _lives++;
        }
    }

    private void OnSwipeLeft()
    {
        if (_isDashing) return;
        AttackNearestEnemy(SwipeDirection.Left);
    }

    private void OnSwipeRight()
    {
        if (_isDashing) return;
        AttackNearestEnemy(SwipeDirection.Right);
    }

    private void OnSwipeDown()
    {
        if (_isDashing) return;
        AttackNearestEnemy(SwipeDirection.Down);
    }

    private void OnSwipeUp()
    {
        if (_isDashing) return;
        AttackNearestEnemy(SwipeDirection.Up);
    }
    
}
