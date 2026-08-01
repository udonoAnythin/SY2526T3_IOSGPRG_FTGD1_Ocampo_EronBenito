using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerStatsScript : EntityStatsScript
{
    public static UnityEvent OnPlayerDeath
    { get => _onPlayerDeath;  }

    [Header("Health UI")]
    [SerializeField] private Image _healthMeter;
    [SerializeField] private Image _healthMeterDamaged;

    [Header("References")]
    [SerializeField] private Camera _camera;


    private static UnityEvent _onPlayerDeath;

    protected override void Awake()
    {
        base.Awake();

        // Assign the onDeath event to the static variable as a reference
        _onPlayerDeath = _onDeath;

        // Add the active player death function to listen to the event
        _onPlayerDeath.AddListener(ActivatePlayerDeath);
    }

    private void LateUpdate()
    {
        AdjustHealthUI();
    }

    private void AdjustHealthUI()
    {
        float newHealth = (float)_currentHealth / (float)_maxHealth;
        _healthMeter.fillAmount = newHealth;

        _healthMeterDamaged.fillAmount = Mathf.Lerp(_healthMeterDamaged.fillAmount, _healthMeter.fillAmount, 0.01f);
    }

    private void ActivatePlayerDeath()
    {
        // Adjust the Health UI one last time
        float newHealth = (float)_currentHealth / (float)_maxHealth;
        _healthMeter.fillAmount = newHealth;
        _healthMeterDamaged.fillAmount = newHealth;

        // Remove Camera as a child
        _camera.transform.SetParent(null, true);

        // Delete player model
        Destroy(gameObject);
    }
}
