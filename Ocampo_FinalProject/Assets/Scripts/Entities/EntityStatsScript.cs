using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityStatsScript : MonoBehaviour
{
    

    [Header("Health Stats")]
    [SerializeField] protected int _maxHealth = 100;
    [SerializeField] protected int _currentHealth;
    [SerializeField] protected UnityEvent _onDeath = new UnityEvent();

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage > 0)
            _currentHealth -= damage;

        if (_currentHealth < 0)
        {
            _currentHealth = 0;
            _onDeath.Invoke();
        }
    }

}
