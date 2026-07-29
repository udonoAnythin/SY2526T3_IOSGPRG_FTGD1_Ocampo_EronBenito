using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsScript : EntityStatsScript
{
    [Header("Health UI")]
    [SerializeField] private Image _healthMeter;
    [SerializeField] private Image _healthMeterDamaged;

    private void LateUpdate()
    {
        AdjustHealthUI();
    }

    private void AdjustHealthUI()
    {
        float newHealth = (float)_currentHealth / (float)_maxHealth;
        _healthMeter.fillAmount = newHealth;

        _healthMeterDamaged.fillAmount = Mathf.Lerp(_healthMeterDamaged.fillAmount, _healthMeter.fillAmount, 0.001f);
    }
}
