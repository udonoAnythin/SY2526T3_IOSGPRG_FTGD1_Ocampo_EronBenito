using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AmmoLoad
{
    public GunType gunType;
    public int currentPackedBullets;
    public int maxLoadedBullets;

    public AmmoLoad(GunType gunType, int maxLoadedBullets, int currentPackedBullets)
    {
        this.gunType = gunType;
        this.maxLoadedBullets = maxLoadedBullets;
        this.currentPackedBullets = currentPackedBullets;
    }
}

public class PlayerGunScript : MonoBehaviour
{

    public GunData Primary
    { get => _currentPrimaryGun; }

    public GunData Secondary
    { get => _currentSecondaryGun; }

    public GunData HeldGun
    { get => _currentHeldGun; }

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

    [Header("Gun Object References")]
    [SerializeField] private SpriteRenderer _gunHolder;

    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private GameObject _bulletPrefab;

    [Header("Gun Variables")]
    [SerializeField] private GunState _gunState;
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
        AmmoLoad newAmmo = _currentBullets[index];
        newAmmo.currentPackedBullets += ammo.AmmoCount;
        if (newAmmo.currentPackedBullets > newAmmo.maxLoadedBullets) newAmmo.currentPackedBullets = newAmmo.maxLoadedBullets;

        // If the current gun has no bullets and is the same type as the picked up ammo
        if (_currentHeldGun != null)
        {
            if (_currentHeldGun.Type == ammo.GunType && _currentHeldGun.currentLoadedBullets == 0)
            {
                _gunState = GunState.Reloading;
            }
        }
    }

    public void CollectGun(GunScript gun)
    {
        switch(gun.GunData.Type)
        {
            case GunType.Pistol:

                //Check if there currently is a pistol
                if (_currentSecondaryGun == null)
                    _currentSecondaryGun = gun.GunData;
                else
                    _currentSecondaryGun.currentLoadedBullets = gun.GunData.currentLoadedBullets;

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

    }

    public void SelectPrimaryGun()
    {
        if (_currentHeldGun == _currentSecondaryGun)
            _gunState = GunState.Idle;

        _currentHeldGun = _currentPrimaryGun;
    }

    public void SelectSecondaryGun()
    {
        if (_currentHeldGun == _currentPrimaryGun)
            _gunState = GunState.Idle;

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

        _gunState = GunState.Firing;
    }

    public void BTN_Release()
    {
        if (_currentHeldGun.Mode == GunMode.Automatic && _gunState != GunState.Reloading)
            _gunState = GunState.Idle;
    }

    private void IdleState()
    {
        if (_currentHeldGun == null) return;

        _currentFireTimer = _currentHeldGun.FireRate;
        _currentReloadTimer = _currentHeldGun.ReloadSpeed;
        _reloadMeter.fillAmount = 1;
        _reloadMeter.enabled = false;
    }

    private void FiringState()
    {

        if (_currentFireTimer == _currentHeldGun.FireRate)
        {

            if (_currentHeldGun == null) 
                return;

            ShootBullet();
            _currentFireTimer -= Time.deltaTime;

            if (_currentHeldGun.currentLoadedBullets <= 0)
            {
                _gunState = GunState.Reloading;
                return;
            }

        }
        else if (_currentFireTimer > 0)
            _currentFireTimer -= Time.deltaTime;
        else
        {
            _currentFireTimer = _currentHeldGun.FireRate;
            if (_currentHeldGun.Mode == GunMode.Semi_Automatic)
                _gunState = GunState.Idle;
        }
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
                _gunState = GunState.Idle;
                return;
            }
        }

        _reloadMeter.enabled = true;
        
        if (_currentReloadTimer > 0)
        {
            _currentReloadTimer -= Time.deltaTime;
            _reloadMeter.fillAmount = 1f - (_currentReloadTimer / _currentHeldGun.ReloadSpeed);
        }
        else
        {
            _reloadMeter.enabled = false;
            Reload();
            _currentReloadTimer = _currentHeldGun.ReloadSpeed;
            _gunState = GunState.Idle;
        }
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
                BulletScript newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity).GetComponent<BulletScript>();
                newBullet.Initialize(_bulletSpawnPoint.transform.TransformDirection(rotatedDirection), _currentHeldGun.Damage);
            }
        }
        else
        {
            BulletScript newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity).GetComponent<BulletScript>();
            newBullet.Initialize(_bulletSpawnPoint.transform.TransformDirection(Vector2.up), _currentHeldGun.Damage);
        }


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
