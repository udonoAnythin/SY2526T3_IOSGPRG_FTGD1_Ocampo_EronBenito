using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityStatsScript : MonoBehaviour
{
    
    public int MaxHealth
    { get => _maxHealth; }

    public int CurrentHealth
    { get => _currentHealth; }

    public string entityName;
    
    [SerializeField] public UnityEvent<EntityStatsScript, EntityStatsScript> onDeath = new UnityEvent<EntityStatsScript, EntityStatsScript>();
    [SerializeField] public UnityEvent onDamaged = new UnityEvent();

    [Header("Health Stats")]
    [SerializeField] protected int _maxHealth = 100;
    [SerializeField] protected int _currentHealth;

    [Header("Entity Visual Components")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _damagedColor;
    [SerializeField] private Color _normalColor;
    [SerializeField] private float _colorChangeSpeed = 0.5f;
    [SerializeField] private GameObject _deathParticles;

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage, EntityStatsScript attacker)
    {
        if (damage > 0)
        {
            _currentHealth -= damage;
            onDamaged.Invoke();
        }

        if (_currentHealth > 0)
        {
            HighlightSpriteDamaged();
        }
        else
        {
            _currentHealth = 0;
            onDeath.Invoke(this, attacker);

            GameObject deathParticles = Instantiate(_deathParticles, transform.position, Quaternion.identity);
            UnityAction particleDestroy = () => Destroy(deathParticles);
            TimerScript.instance.StartCoroutine(TimerScript.instance.CO_ExecuteAfterTimer(1, particleDestroy));
        }
    }

    private void HighlightSpriteDamaged()
    {
        UnityAction highlightToDamaged = () =>
        {
            if (_spriteRenderer == null) return;

            Color newColor = Color.Lerp(_spriteRenderer.color, _damagedColor, 0.1f);
            _spriteRenderer.color = newColor;
        };

        UnityAction changeToNormal = () =>
        {
            if (_spriteRenderer == null) return;

            Color newColor = Color.Lerp(_spriteRenderer.color, _normalColor, 0.1f);
            _spriteRenderer.color = newColor;
        };

        UnityAction highlightToNormal = () => TimerScript.instance.StartCoroutine(TimerScript.instance.CO_ExecuteDuringTimer(_colorChangeSpeed, changeToNormal));

        TimerScript.instance.StartCoroutine(TimerScript.instance.CO_ExecuteDuringTimer(_colorChangeSpeed, highlightToDamaged, highlightToNormal));
    }

}
