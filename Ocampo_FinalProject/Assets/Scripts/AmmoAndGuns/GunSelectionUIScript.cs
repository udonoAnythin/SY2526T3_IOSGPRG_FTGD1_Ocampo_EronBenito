using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunSelectionUIScript : MonoBehaviour
{

    [SerializeField] private PlayerGunScript _playerStats;

    [SerializeField] private Color _unselected;
    [SerializeField] private Color _selected;

    [SerializeField] private Image _primaryButtonImage;
    [SerializeField] private Image _secondaryButtonImage;

    [SerializeField] private TextMeshProUGUI _numberText1;
    [SerializeField] private TextMeshProUGUI _numberText2;

    [SerializeField] private TextMeshProUGUI _primaryText;
    [SerializeField] private TextMeshProUGUI _secondaryText;

    [SerializeField] private Image _primaryImage;
    [SerializeField] private Image _secondaryImage;


    public void LateUpdate()
    {
        UpdateImageUI();
    }

    public void BTN_PrimarySelect()
    {
        Debug.Log("Primary Selected");

        _playerStats.SelectPrimaryGun();
    }

    public void BTN_SecondarySelect()
    {
        Debug.Log("Secondary Selected");

        _playerStats.SelectSecondaryGun();

    }

    private void UpdateImageUI()
    {
        if (_playerStats.Primary == null)
            _primaryImage.enabled = false;
        else
        {
            _primaryImage.enabled = true;
            _primaryImage.sprite = _playerStats.Primary.GunWorldSprite;
        }

        if (_playerStats.Secondary == null)
            _secondaryImage.enabled = false;
        else
        {
            _secondaryImage.enabled = true;
            _secondaryImage.sprite = _playerStats.Secondary.GunWorldSprite;
        }

        if (_playerStats.HeldGun != null)
        {
            if (_playerStats.HeldGun == _playerStats.Primary)
            {
                _primaryButtonImage.color = _selected;
                _secondaryButtonImage.color = _unselected;

                _numberText1.color = Color.black;
                _numberText2.color = Color.white;

                _primaryText.color = Color.black;
                _secondaryText.color = Color.white;
            }
            else if (_playerStats.HeldGun == _playerStats.Secondary)
            {
                _secondaryButtonImage.color = _selected;
                _primaryButtonImage.color = _unselected;

                _numberText2.color = Color.black;
                _numberText1.color = Color.white;

                _secondaryText.color = Color.black;
                _primaryText.color = Color.white;

                _playerStats.SelectSecondaryGun();
            }
        }

    }

}
