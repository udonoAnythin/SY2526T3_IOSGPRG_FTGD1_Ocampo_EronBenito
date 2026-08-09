using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class AmmoLoad
{
    public GunType gunType;
    public int currentPackedBullets;
    public int maxLoadableBullets;

    public AmmoLoad(GunType gunType, int maxLoadedBullets, int currentPackedBullets)
    {
        this.gunType = gunType;
        this.maxLoadableBullets = maxLoadedBullets;
        this.currentPackedBullets = currentPackedBullets;
    }
}

public class PlayerShootScript : MonoBehaviour
{

    public GunData Primary
    { get => _currentPrimaryGun; }

    public GunData Secondary
    { get => _currentSecondaryGun; }

    public GunData HeldGun
    { get => _currentHeldGun; }

    [Header("Camera References")]
    [SerializeField] private float _camShootShakeTimer = 0.15f;
    [SerializeField] private float _camShootShakeIntensity = 0.1f;

    [Header("Gun Stats")]
    [SerializeField] private List<AmmoLoad> _currentBullets = new List<AmmoLoad>();
    [SerializeField] private GunData _currentPrimaryGun;
    [SerializeField] private GunData _currentSecondaryGun;
    [SerializeField] private GunData _currentHeldGun;

    [Header("Gun UI")]
    [SerializeField] private TextMeshProUGUI _currentAmmoCountText;
    [SerializeField] private TextMeshProUGUI _totalAmmoCountText;
    [SerializeField] private List<TextMeshProUGUI> _totalAmmoCountAll = new List<TextMeshProUGUI>();
    [SerializeField] private Image _reloadMeter;
    [SerializeField] private GameObject _totalRocketAmmoUI;

    [Header("Gun Object References")]
    [SerializeField] private SpriteRenderer _gunHolder;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private ParticleSystem _muzzleParticles;

    [Header("Gun Variables")]
    [SerializeField] private GunState _gunState;
    [SerializeField] private Coroutine _fireCoroutine;
    private float _currentFireTimer;
    private float _currentReloadTimer;

    private void Awake()
    {
        // Set Held Gun
        _currentHeldGun = _currentPrimaryGun;
        _reloadMeter.enabled = false;
    }

    private void Update()
    {
        switch(_gunState)
        {
            case GunState.Firing:

                FiringState();
                break;

            case GunState.Reloading:

                ReloadingState();
                break;

            case GunState.Idle:

                IdleState();
                break;
        }
    }

    private void LateUpdate()
    {
        AdjustAmmoUI();

        AdjustGunSprite();
    }

    public void CollectBullets(AmmoScript ammo)
    {
        int index = _currentBullets.FindIndex((ammoLoad) => (ammoLoad.gunType == ammo.GunType));
        
        // Load the bullets
        _currentBullets[index].currentPackedBullets += ammo.AmmoCount;
        if (_currentBullets[index].currentPackedBullets > _currentBullets[index].maxLoadableBullets) _currentBullets[index].currentPackedBullets = _currentBullets[index].maxLoadableBullets;

        // If the current gun has no bullets and is the same type as the picked up ammo
        if (_currentHeldGun != null)
        {
            if (_currentHeldGun.Type == ammo.GunType && _currentHeldGun.currentLoadedBullets == 0)
            {
                EnterReloadingState();
            }
        }
    }

    public void CollectGun(GunScript gun)
    {
        switch(gun.GunData.Type)
        {
            case GunType.RocketLauncher:
            case GunType.Pistol:

                if (gun.GunData.Type == GunType.RocketLauncher)
                    _totalRocketAmmoUI.SetActive(true);

                //Check if there currently is a pistol
                if (_currentSecondaryGun == null)
                    _currentSecondaryGun = gun.GunData;
                else
                {
                    if (_currentSecondaryGun.Type != gun.GunData.Type)
                    {
                        _currentSecondaryGun = gun.GunData;
                        _currentHeldGun = _currentSecondaryGun;
                    }
                    else
                    {
                        _currentSecondaryGun.currentLoadedBullets = gun.GunData.currentLoadedBullets;
                    }
                }
                break;

            case GunType.AutomaticRifle:
            case GunType.Shotgun:
            
                //Check if there currently is a primary
                if (_currentPrimaryGun == null)
                    _currentPrimaryGun = gun.GunData;
                else
                {
                    if (_currentPrimaryGun.Type != gun.GunData.Type)
                    {
                        _currentPrimaryGun = gun.GunData;
                        _currentHeldGun = _currentPrimaryGun;
                    }
                    else
                    {
                        _currentPrimaryGun.currentLoadedBullets = gun.GunData.currentLoadedBullets;
                    }
                }
                break;

        }

        if (_currentHeldGun == null)
        {
            _currentHeldGun = gun.GunData;
        }
         _currentFireTimer = _currentHeldGun.FireRate;
        

    }

    public void SelectPrimaryGun()
    {
        if (_currentHeldGun == _currentSecondaryGun)
            EnterIdleState();

        _currentHeldGun = _currentPrimaryGun;
    }

    public void SelectSecondaryGun()
    {
        if (_currentHeldGun == _currentPrimaryGun)
            EnterIdleState();

        _currentHeldGun = _currentSecondaryGun;
    }

    

    private void AdjustAmmoUI()
    {
        // Current Loaded Bullets
        if (_currentHeldGun != null)
        {

            _currentAmmoCountText.text = _currentHeldGun.currentLoadedBullets.ToString();

            if (_currentHeldGun.currentLoadedBullets <= Mathf.RoundToInt(_currentHeldGun.MagSize * 0.1f))
                _currentAmmoCountText.color = Color.red;
            else
                _currentAmmoCountText.color = Color.white;
        }
        else
        {
            _currentAmmoCountText.text = "0";
        }

        //Total Collected Bullets
        if (_currentHeldGun != null)
            _totalAmmoCountText.text = _currentBullets.Find((_ammoLoad) => (_ammoLoad.gunType == _currentHeldGun.Type)).currentPackedBullets.ToString();
        else
            _totalAmmoCountText.text = "0";

        for (int i = 0; i < _totalAmmoCountAll.Count; i++)
        {
            _totalAmmoCountAll[i].text = _currentBullets[i].currentPackedBullets.ToString();
        }

    }

    private void AdjustGunSprite()
    {
        if (_currentHeldGun != null)
            _gunHolder.sprite = _currentHeldGun.GunHoldSprite;
        else
            _gunHolder.sprite = null;
    }

    public void BTN_Fire()
    {
        if (_currentHeldGun == null) 
            return;
        if (_currentHeldGun.currentLoadedBullets == 0)
            return;

        EnterFiringState();
    }

    public void BTN_Release()
    {
        if (_currentHeldGun != null)
            if (_currentHeldGun.Mode == GunMode.Automatic && _gunState != GunState.Reloading)
                EnterIdleState();
    }

    private void IdleState()
    {
        // Nothing is done
    }

    private void FiringState()
    {
        UnityAction firingAction = () =>
        {
            if (_currentFireTimer == _currentHeldGun.FireRate)
            {

                if (_currentHeldGun == null)
                    return;

                ShootBullet();
                _currentFireTimer -= Time.deltaTime;

                // Reload immediately after firing the last bullet
                if (_currentHeldGun.currentLoadedBullets <= 0)
                {
                    EnterReloadingState();
                    return;
                }

            }
            else if (_currentFireTimer > 0)
                _currentFireTimer -= Time.deltaTime;

        };

        UnityAction resetAction = () =>
        {
            _currentFireTimer = _currentHeldGun.FireRate;

            // Only fire a Semi-Auto via press, not hold by switching states after the fire cooldown
            if (_currentHeldGun.Mode == GunMode.Semi_Automatic)
            {
                EnterIdleState();
            }
            
            _fireCoroutine = null;
        };

        if (_fireCoroutine == null)
            _fireCoroutine = StartCoroutine(TimerScript.instance.CO_ExecuteDuringTimer(_currentHeldGun.FireRate, firingAction, resetAction));
    }

    private void ReloadingState()
    {

        // Check first if there are any bullets to reload
        if (_currentHeldGun != null)
        {
            int index = _currentBullets.FindIndex((ammoLoad) => (ammoLoad.gunType == _currentHeldGun.Type));
            AmmoLoad newAmmo = _currentBullets[index];

            if (newAmmo.currentPackedBullets <= 0)
            {
                EnterIdleState();
                return;
            }
        }

        
    }

    private void EnterIdleState()
    {
        _gunState = GunState.Idle;

        if (_currentHeldGun == null) return;

        //_currentFireTimer = _currentHeldGun.FireRate;
        _currentReloadTimer = _currentHeldGun.ReloadSpeed;
        _reloadMeter.fillAmount = 0;
        _reloadMeter.enabled = false;
    }

    private void EnterFiringState()
    {
        _gunState = GunState.Firing;

        UnityAction firingAction = () =>
        {
            if (_currentFireTimer == _currentHeldGun.FireRate)
            {

                if (_currentHeldGun == null)
                    return;

                ShootBullet();
                _currentFireTimer -= Time.deltaTime;

                // Reload immediately after firing the last bullet
                if (_currentHeldGun.currentLoadedBullets <= 0)
                {
                    EnterReloadingState();
                    return;
                }

            }
            else if (_currentFireTimer > 0)
                _currentFireTimer -= Time.deltaTime;

        };

        UnityAction resetAction = () =>
        {
            _currentFireTimer = _currentHeldGun.FireRate;

            // Only fire a Semi-Auto via press, not hold by switching states after the fire cooldown
            if (_currentHeldGun.Mode == GunMode.Semi_Automatic)
            {
                EnterIdleState();
            }
            _fireCoroutine = null;
        };

        if (_fireCoroutine == null)
            _fireCoroutine = StartCoroutine(TimerScript.instance.CO_ExecuteDuringTimer(_currentHeldGun.FireRate, firingAction, resetAction));
    }

    private void EnterReloadingState()
    {
        _gunState = GunState.Reloading;

        _reloadMeter.enabled = true;

        UnityAction calculateMeterFillAmount = () =>
        {
            _currentReloadTimer -= Time.deltaTime;
            _reloadMeter.fillAmount = 1f - (_currentReloadTimer / _currentHeldGun.ReloadSpeed);
        };

        UnityAction reloadGunAndResetState = () => 
        {
            Reload();
            _reloadMeter.fillAmount = 0;
            _reloadMeter.enabled = false;
            _currentReloadTimer = _currentHeldGun.ReloadSpeed;
            EnterIdleState();
        };

        StartCoroutine(TimerScript.instance.CO_ExecuteDuringTimer(_currentHeldGun.ReloadSpeed, calculateMeterFillAmount, reloadGunAndResetState));
    }

    private void ShootBullet()
    {
        _currentHeldGun.currentLoadedBullets--;
        

        if (_currentHeldGun.Type == GunType.Shotgun)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = Random.Range(-_currentHeldGun.ShotgunArcAngle, _currentHeldGun.ShotgunArcAngle);
                Vector2 rotatedDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;
                BulletScript newBullet = Instantiate(_currentHeldGun.BulletPrefab, _bulletSpawnPoint.position, Quaternion.identity).GetComponent<BulletScript>();
                newBullet.Initialize(_bulletSpawnPoint.transform.TransformDirection(rotatedDirection), _currentHeldGun.Damage, gameObject.GetComponent<EntityStatsScript>());
            }
        }
        else
        {
            BulletScript newBullet = Instantiate(_currentHeldGun.BulletPrefab, _bulletSpawnPoint.position, Quaternion.identity).GetComponent<BulletScript>();
            newBullet.Initialize(_bulletSpawnPoint.transform.TransformDirection(Vector2.up), _currentHeldGun.Damage, gameObject.GetComponent<EntityStatsScript>());
        }

        CameraScript.instance.ShakeCamera(_camShootShakeTimer, _camShootShakeIntensity);
        _muzzleParticles.Play();

    }


    private void Reload()
    {
        if (_currentHeldGun == null)
            return;

        // Get the ammoload with the same type as the current gun
        int index = _currentBullets.FindIndex((ammoLoad) => (ammoLoad.gunType == _currentHeldGun.Type));
        AmmoLoad newAmmo = _currentBullets[index];

        if (newAmmo.currentPackedBullets == 0)
            return;

        // Add the ammo to the gun
        if (newAmmo.currentPackedBullets < _currentHeldGun.MagSize)
            _currentHeldGun.currentLoadedBullets = newAmmo.currentPackedBullets;
        else
            _currentHeldGun.currentLoadedBullets = _currentHeldGun.MagSize;

        // Deduct ammo by the mag size of the gun
        newAmmo.currentPackedBullets -= _currentHeldGun.MagSize;
        if (newAmmo.currentPackedBullets < 0) 
            newAmmo.currentPackedBullets = 0;

    }


}
