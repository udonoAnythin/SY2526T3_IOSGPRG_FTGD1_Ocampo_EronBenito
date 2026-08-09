using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletScript : MonoBehaviour
{

    public int Damage
    { get => _damage; }

    [Header("Bullet Variables")]
    [SerializeField] protected float _speed;
    [SerializeField] protected int _damage;
    [SerializeField] protected float _destroyTimer;
    [SerializeField] protected EntityStatsScript owner;

    [Header("Particle References")]
    [SerializeField] protected GameObject _entityHitParticles;
    [SerializeField] protected GameObject _obstacleHitParticles;

    [Header("Components")]
    [SerializeField] protected Rigidbody2D _rb;

    protected virtual void Awake()
    {
        Destroy(gameObject, _destroyTimer);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Stops checking if it collides with other bullets
        if (collision.gameObject.GetComponent<BulletScript>() != null) return;

        // Damages entities
        EntityStatsScript entity = collision.gameObject.GetComponent<EntityStatsScript>();
        if (entity != null)
        {
            entity.TakeDamage(Damage, owner);

            // Pause the game for 0.1s after hitting an enemy
            if (owner is PlayerStatsScript)
            {
                UnityAction pauseGame = () => Time.timeScale = 0;
                UnityAction resumeGame = () => Time.timeScale = 1;

                TimerScript.instance.StartCoroutine(TimerScript.instance.CO_ExecuteInRealTime(0.1f, pauseGame, resumeGame));
            }

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

        Destroy(gameObject);
    }

    public void Initialize(Vector2 direction, int damage, EntityStatsScript owner)
    {
        _rb.velocity = direction * _speed;
        _damage = damage;
        this.owner = owner;
    }



}
