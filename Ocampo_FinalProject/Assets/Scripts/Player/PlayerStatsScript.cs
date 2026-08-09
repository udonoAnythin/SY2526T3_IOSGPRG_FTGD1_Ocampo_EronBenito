using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerStatsScript : EntityStatsScript
{
    

    [Header("Health UI")]
    [SerializeField] private Image _healthMeter;
    [SerializeField] private Image _healthMeterDamaged;

    [Header("Camera References")]
    [SerializeField] private float _camDamageShakeTimer = 0.3f;
    [SerializeField] private float _camDamageShakeIntensity = 0.5f;

    protected override void Awake()
    {
        base.Awake();

        // Add the active player death function to listen to the event
        onDeath.AddListener(ActivatePlayerDeath);
        onDamaged.AddListener(ShakeCameraOnDamage);

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

    private void ActivatePlayerDeath(EntityStatsScript victim, EntityStatsScript killer)
    {
        // Adjust the Health UI one last time
        float newHealth = (float)_currentHealth / (float)_maxHealth;
        _healthMeter.fillAmount = newHealth;
        _healthMeterDamaged.fillAmount = newHealth;

        // Remove Camera as a child
        CameraScript.instance.gameObject.transform.SetParent(null, true);

        // Delete player model
        Destroy(gameObject);
    }

    private void ShakeCameraOnDamage()
    {
        if (_currentHealth > 0)
            CameraScript.instance.ShakeCamera(_camDamageShakeTimer, _camDamageShakeIntensity);
    }
    
}
