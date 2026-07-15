using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatsScript : MonoBehaviour
{
    [Header("Health Stats")]
    [SerializeField] protected int _maxHealth = 100;
    [SerializeField] protected int _currentHealth;

    

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage > 0)
        {
            _currentHealth -= damage;
        }
    }

}
