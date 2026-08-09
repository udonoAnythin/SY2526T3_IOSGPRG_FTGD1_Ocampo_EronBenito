using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealthMeterScript : MonoBehaviour
{
    [Header("Health Bar References")]
    [SerializeField] private EnemyStatsScript _enemyStats;
    [SerializeField] private GameObject _healthBarObject;
    [SerializeField] private SpriteRenderer _healthBarBlock;
    [SerializeField] private SpriteRenderer _healthBarFill;
    [SerializeField] private SpriteRenderer _healthBarFillDamage;

    [Header("Health Bar Variables")]
    [SerializeField] private float _maxHealthBarShowTimer = 3f;
    [SerializeField] private float _healthBarFadeTimer = 0.5f;
    [SerializeField] private float _healthBarAlpha = 0.75f;

    private Coroutine _displayHealthbarCoroutine = null;
    private float _maxHealthMeterWidth;
    private float _mainHealthbarLocalPositionX;


    private void Awake()
    {
        _maxHealthMeterWidth = _healthBarFill.size.x;
        _mainHealthbarLocalPositionX = _healthBarFill.gameObject.transform.localPosition.x;

        _enemyStats.onDamaged.AddListener(DisplayHealthUI);
        _healthBarObject.SetActive(false);
    }

    private void DisplayHealthUI()
    {
        _healthBarObject.SetActive(true);

        AdjustEnemyHealthUI();
        
        UnityAction fadeOutUI = () =>
        {
            if (_healthBarBlock == null) return;

            Color newFillColor = _healthBarBlock.color;
            newFillColor.a = 0;
            _healthBarBlock.color = Color.Lerp(_healthBarBlock.color, newFillColor, 0.1f);
            
            Color newFillColor2 = _healthBarFill.color;
            newFillColor2.a = 0;
            _healthBarFill.color = Color.Lerp(_healthBarFill.color, newFillColor2, 0.1f);

            Color newFillColor3 = _healthBarFillDamage.color;
            newFillColor3.a = 0;
            _healthBarFillDamage.color = Color.Lerp(_healthBarFillDamage.color, newFillColor3, 0.1f);

        };

        UnityAction removeUI = () =>
        {
            if (_healthBarBlock == null) return;

            Color newFillColor = _healthBarBlock.color;
            newFillColor.a = _healthBarAlpha;
            _healthBarBlock.color = newFillColor;

            Color newFillColor2 = _healthBarFill.color;
            newFillColor2.a = 1;
            _healthBarFill.color = newFillColor2;

            Color newFillColor3 = _healthBarFillDamage.color;
            newFillColor3.a = _healthBarAlpha;
            _healthBarFillDamage.color = newFillColor3;

            _healthBarObject.SetActive(false);
        };

        if (_displayHealthbarCoroutine != null ) StopCoroutine( _displayHealthbarCoroutine );

        UnityAction fadeOut = () => TimerScript.instance.StartCoroutine(TimerScript.instance.CO_ExecuteDuringTimer(_healthBarFadeTimer, fadeOutUI, removeUI));

        _displayHealthbarCoroutine = StartCoroutine(TimerScript.instance.CO_ExecuteAfterTimer(_maxHealthBarShowTimer, fadeOut));
    }

    private void AdjustEnemyHealthUI()
    {
        // Set meter progress
        float percentage = (float)_enemyStats.CurrentHealth / (float)_enemyStats.MaxHealth;
        if (_healthBarFill == null) return;

        // Set meter size and position
        _healthBarFill.size = new Vector2(_maxHealthMeterWidth * percentage, _healthBarFill.size.y);
        _healthBarFill.transform.localPosition = new Vector3(_mainHealthbarLocalPositionX - (_maxHealthMeterWidth * (1 - percentage) / 2), _healthBarFill.transform.localPosition.y, _healthBarFill.transform.localPosition.z);

        // Set damage meter size and position (meter that chases after the true damage meter)
        UnityAction adjustDamageMeter = () =>
        {
            if (_healthBarFill == null) return;
            _healthBarFillDamage.size = Vector2.Lerp(_healthBarFillDamage.size, _healthBarFill.size, 0.01f);
            _healthBarFillDamage.transform.localPosition = Vector3.Lerp(_healthBarFillDamage.transform.localPosition, _healthBarFill.transform.localPosition, 0.01f);
        };

        TimerScript.instance.StartCoroutine(TimerScript.instance.CO_ExecuteDuringTimer(1f, adjustDamageMeter));
    }
}
