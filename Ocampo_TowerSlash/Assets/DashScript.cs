using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DashScript : MonoBehaviour
{

    [SerializeField] private SwipeDetectionScript _swipeDetectionScript;

    [Header("Objects Affected By Dash")]
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private LevelManagerScript _levelManagerScript;

    [Header("Dash Variables")]
    [SerializeField] private Image _dashMeter;
    [SerializeField] private float _tapAddedValue = 2f;
    [SerializeField] private float _dashValue = 7;
    [SerializeField] private float _dashMaxValue = 10;
    [SerializeField] private float _speedMultiplier = 5;

    private void OnEnable()
    {
        _swipeDetectionScript.Tapped.AddListener(TapLeap);
    }

    private void OnDisable()
    {
        _swipeDetectionScript.Tapped.RemoveListener(TapLeap);
    }

    private void Update()
    {
        _dashMeter.fillAmount = _dashValue / _dashMaxValue;

        if (!_playerScript.IsDashing && _dashValue < _dashMaxValue) 
        { 
            _dashValue += Time.deltaTime;
        }
        
    }
    public void Dash()
    {

        if (_dashValue >= _dashMaxValue)
        {
            UnityAction dash = () => _levelManagerScript.ChangeSpeeds(_speedMultiplier);
            dash += () => _playerScript.SetDash(true);
            dash += () => _dashValue -= Time.deltaTime;
            dash += () => _swipeDetectionScript.Tapped.RemoveListener(TapLeap);

            UnityAction revertedSpeed = () => _levelManagerScript.RevertSpeeds();
            revertedSpeed += () => _playerScript.SetDash(false);
            revertedSpeed += () => _swipeDetectionScript.Tapped.AddListener(TapLeap);

            StartCoroutine(TimerScript.Instance.CO_ExecuteDuringTimer(dash, _dashMaxValue, revertedSpeed));
        }
        
    }

    private void TapLeap()
    {
        if (_levelManagerScript.CheckEnemyCount() <= 0)
        {
            UnityAction leap = () => _levelManagerScript.ChangeSpeeds(_speedMultiplier);

            UnityAction revertedSpeed = () => _levelManagerScript.RevertSpeeds();
            revertedSpeed += () => _dashValue += _tapAddedValue;
            
            StartCoroutine(TimerScript.Instance.CO_ExecuteDuringTimer(leap, 0.25f, revertedSpeed));
        }
    }


}
