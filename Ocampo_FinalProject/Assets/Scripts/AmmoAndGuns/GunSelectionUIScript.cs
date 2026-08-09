using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunSelectionUIScript : MonoBehaviour
{

    [SerializeField] private PlayerShootScript _playerShootScript;

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

        _playerShootScript.SelectPrimaryGun();
    }

    public void BTN_SecondarySelect()
    {
        Debug.Log("Secondary Selected");

        _playerShootScript.SelectSecondaryGun();

    }

    private void UpdateImageUI()
    {
        if (_playerShootScript.Primary == null)
            _primaryImage.enabled = false;
        else
        {
            _primaryImage.enabled = true;
            _primaryImage.sprite = _playerShootScript.Primary.GunWorldSprite;
        }

        if (_playerShootScript.Secondary == null)
            _secondaryImage.enabled = false;
        else
        {
            _secondaryImage.enabled = true;
            _secondaryImage.sprite = _playerShootScript.Secondary.GunWorldSprite;
        }

        if (_playerShootScript.HeldGun != null)
        {
            if (_playerShootScript.HeldGun == _playerShootScript.Primary)
            {
                _primaryButtonImage.color = _selected;
                _secondaryButtonImage.color = _unselected;

                _numberText1.color = Color.black;
                _numberText2.color = Color.white;

                _primaryText.color = Color.black;
                _secondaryText.color = Color.white;
            }
            else if (_playerShootScript.HeldGun == _playerShootScript.Secondary)
            {
                _secondaryButtonImage.color = _selected;
                _primaryButtonImage.color = _unselected;

                _numberText2.color = Color.black;
                _numberText1.color = Color.white;

                _secondaryText.color = Color.black;
                _primaryText.color = Color.white;

                _playerShootScript.SelectSecondaryGun();
            }
        }

    }

}
