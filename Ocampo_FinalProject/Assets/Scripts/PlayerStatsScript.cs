using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsScript : EntityStatsScript
{


    [Header("Gun Stats")]
    [SerializeField] private List<int> _currentLoadedBullets = new List<int>(); // 0 for 9mm, 1 for 5.56 mm, 2 for 12 gauge
    [SerializeField] private int _currentBulletIndex = 0;

    [SerializeField] private GunScript _currentGun;

    [Header("Health UI")]
    [SerializeField] private Image _healthMeter;
    [SerializeField] private Image _healthMeterDamaged;

    [Header("Gun UI")]
    [SerializeField] private TextMeshProUGUI _currentAmmoCount;
    [SerializeField] private TextMeshProUGUI _totalAmmoCount;

    private void Awake()
    {
        // Initialize Loaded Bullets
        for (int i = 0; i < 3; i++)
            _currentLoadedBullets.Add(0);
    }

    private void LateUpdate()
    {
        AdjustHealthUI();

        AdjustAmmoUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.GetComponent<AmmoScript>() != null)
        {
            AmmoScript ammo = other.GetComponent<AmmoScript>();
            CollectBullets(ammo);

            Destroy(other.gameObject);
        }

    }

    private void AdjustHealthUI()
    {
        float newHealth = (float)_currentHealth / (float)_maxHealth;
        _healthMeter.fillAmount = newHealth;

        _healthMeterDamaged.fillAmount = Mathf.Lerp(_healthMeterDamaged.fillAmount, _healthMeter.fillAmount, 0.001f);
    }

    private void AdjustAmmoUI()
    {
        // Current Loaded Bullets
        if (_currentGun != null)
        {
            _currentBulletIndex = (int)_currentGun.Type;
            _currentAmmoCount.text = _currentGun.currentLoadedBullets.ToString();
        }
        else
        {
            _currentAmmoCount.text = "0";
        }

        //Total Collected Bullets
        _totalAmmoCount.text = _currentLoadedBullets[_currentBulletIndex].ToString();

    }

    private void CollectBullets(AmmoScript ammo)
    {
        _currentLoadedBullets[(int)ammo.GunType] += ammo.AmmoCount;
    }

}
