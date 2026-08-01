using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public int Damage
    { get => _damage; }    

    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private float _destroyTimer;

    [SerializeField] private Rigidbody2D _rb;

    private void Awake()
    {
        Destroy(gameObject, _destroyTimer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Stops checking if it collides with other bullets
        if (collision.gameObject.GetComponent<BulletScript>() != null) return;

        // Damages entities
        EntityStatsScript entity = collision.gameObject.GetComponent<EntityStatsScript>();
        if (entity != null)
        {
            Debug.Log(entity);
            entity.TakeDamage(Damage);
        }

        Destroy(gameObject);
    }

    public void Initialize(Vector2 direction, int damage)
    {
        _rb.velocity = direction * _speed;
        _damage = damage;
    }



}
