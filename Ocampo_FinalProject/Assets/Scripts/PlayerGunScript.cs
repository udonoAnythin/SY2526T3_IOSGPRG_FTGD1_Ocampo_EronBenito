using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct AmmoLoad
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

    [Header("Gun Stats")]
    [SerializeField] private List<AmmoLoad> _currentBullets = new List<AmmoLoad>();

    [SerializeField] private GunData _currentPrimaryGun;
    [SerializeField] private GunData _currentSecondaryGun;

    [SerializeField] private GunData _currentHeldGun;

    [Header("Gun UI")]
    [SerializeField] private TextMeshProUGUI _currentAmmoCountText;
    [SerializeField] private TextMeshProUGUI _totalAmmoCountText;
    [SerializeField] private List<TextMeshProUGUI> _totalAmmoCountAll = new List<TextMeshProUGUI>();

    [Header("Gun Sprites")]
    [SerializeField] private SpriteRenderer _gunHolder;

    private void Awake()
    {
        // Set Held Gun
        _currentHeldGun = _currentPrimaryGun;
    }

    private void LateUpdate()
    {
        AdjustAmmoUI();
    }

    public void CollectBullets(AmmoScript ammo)
    {
        Debug.Log("Bullets Collected!");
        int index = _currentBullets.FindIndex((ammoLoad) => (ammoLoad.gunType == ammo.GunType));
        
        AmmoLoad newAmmo = _currentBullets[index];
        newAmmo.currentPackedBullets += ammo.AmmoCount;
        if (newAmmo.currentPackedBullets > newAmmo.maxLoadedBullets) newAmmo.currentPackedBullets = newAmmo.maxLoadedBullets;
        _currentBullets[index] = newAmmo;
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
                        _currentPrimaryGun = gun.GunData;
                    else
                        _currentPrimaryGun.currentLoadedBullets = gun.GunData.currentLoadedBullets;
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
        _currentHeldGun = _currentPrimaryGun;
    }

    public void SelectSecondaryGun()
    {
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

    }

    public void BTN_Reload()
    {

    }

    

}
