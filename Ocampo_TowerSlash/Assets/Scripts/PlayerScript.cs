using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Character
{
    Default,
    Tank,
    Speed
}

public class PlayerScript : MonoBehaviour
{
    public float Lives
    {
        get => _lives;
        set => _lives = value;
    }

    public bool IsDashing
    {
        get => _isDashing;
    }

    public Character PlayerCharacter
    {
        get => _playerCharacter;
        set => _playerCharacter = value;
    }

    public UnityEvent PlayerDamaged
    {
        get => _playerDamage;
    }

    public UnityEvent EnemyKilled
    {
        get => _enemyKilled;
    }

    [Header("Variables and References")]
    [SerializeField] private float _lives = 3;
    [SerializeField] private Character _playerCharacter = Character.Default;
    [SerializeField] private SwipeDetectionScript _swipeDetectionScript;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Death Variables")]
    [SerializeField] private GameObject _deathParticles;
    [SerializeField] private AudioClip _deathSFX;

    [Header("Events")]
    [SerializeField] private UnityEvent _playerDamage = new UnityEvent();
    [SerializeField] private UnityEvent _enemyKilled = new UnityEvent();

    private Queue<EnemyScript> _enemies = new Queue<EnemyScript>();
    private bool _isDashing = false;

    private void OnEnable()
    {
        _swipeDetectionScript.SwipedLeft.AddListener(OnSwipeLeft);
        _swipeDetectionScript.SwipedRight.AddListener(OnSwipeRight);
        _swipeDetectionScript.SwipedDown.AddListener(OnSwipeDown);
        _swipeDetectionScript.SwipedUp.AddListener(OnSwipeUp);

        _enemyKilled.AddListener(DoAttackAnimation);
    }

    private void OnDisable()
    {
        _swipeDetectionScript.SwipedLeft.RemoveListener(OnSwipeLeft);
        _swipeDetectionScript.SwipedRight.RemoveListener(OnSwipeRight);
        _swipeDetectionScript.SwipedDown.RemoveListener(OnSwipeDown);
        _swipeDetectionScript.SwipedUp.RemoveListener(OnSwipeUp);

        _enemyKilled.RemoveListener(DoAttackAnimation);
    }

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
            EnemyScript enemy = _enemies.Dequeue();
            enemy.TakeDamage();
            _enemyKilled.Invoke();

            if (!_isDashing)
            {
                //Reduce Lives
                TakeDamage();

            }
        }
    }

    public void SetCharacter(Character character)
    {
        _playerCharacter = character;

        switch(_playerCharacter)
        {
            case Character.Default:

                _animator.Play("DefaultPlayerRun");
                break;

            case Character.Tank:

                _animator.Play("TankPlayerRun");
                break;

            case Character.Speed:

                _animator.Play("SpeederPlayerRun");
                break;
        }
    }
    public void SetDash(bool isDashing)
    {
        _isDashing = isDashing;
    }

    public void SetSpriteEnabled(bool isEnabled)
    {
        _spriteRenderer.enabled = isEnabled;
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

                _enemyKilled.Invoke();
                


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

        //Spawn Particles
        Instantiate(_deathParticles, transform.position, Quaternion.identity);

        SoundManagerScript.Instance.PlaySFX(_deathSFX);
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

    private void DoAttackAnimation()
    {
        switch (_playerCharacter)
        {
            case Character.Default:

                _animator.Play("DefaultPlayerAttack");
                break;

            case Character.Tank:

                _animator.Play("TankPlayerAttack");
                break;

            case Character.Speed:

                _animator.Play("SpeederPlayerAttack");
                break;
        }
    }
    
}
