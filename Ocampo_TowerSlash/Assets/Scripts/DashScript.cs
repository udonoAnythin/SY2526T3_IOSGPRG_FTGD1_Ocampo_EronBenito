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
    [SerializeField] private Image _dashMeterBackground;
    [SerializeField] private float _dashValue = 0;
    [SerializeField] private float _dashMaxValue = 30;
    [SerializeField] private float _speedMultiplier = 5;
    [SerializeField] private GameObject _dashButton;
    [SerializeField] private GameObject _dashParticles;

    [Header("Tap Dash Variables")]
    [SerializeField] private float _tapAddedValue = 2f;
    [SerializeField] private float _tapSpeedTimer = 0.25f;

    [Header("Dash to Enemy Variables")]
    [SerializeField] private float _dashAddedOnEnemyKill = 0.05f;
    [SerializeField] private float _enemyDashSpeedTimer = 0.0002f;
    [SerializeField] private float _enemyDashSpeed = 8f;
    [SerializeField] private float _enemyDashPauseTimer = 0.5f;

    private bool _playerIsDead = false;

    private void OnEnable()
    {
        _swipeDetectionScript.Tapped?.AddListener(TapLeap);

        _playerScript.EnemyKilled?.AddListener(OnEnemyKilled);
        _playerScript.PlayerDamaged?.AddListener(OnPlayerDamaged);
    }

    private void OnDisable()
    {
        _swipeDetectionScript.Tapped?.RemoveListener(TapLeap);

        _playerScript.EnemyKilled?.RemoveListener(OnEnemyKilled);
        _playerScript.PlayerDamaged?.RemoveListener(OnPlayerDamaged);
    }

    private void Update()
    {
        _dashMeter.fillAmount = _dashValue / _dashMaxValue;

        if (!(!_playerScript.IsDashing || !_playerIsDead || _swipeDetectionScript.CheckIfInputEnabled())) 
        { 
            AddDashValue(Time.deltaTime);
            Debug.Log("Adding");
        }
        
        if (_playerIsDead)
        {
            StopAllCoroutines();
        }

        _dashButton.SetActive(_dashValue >= _dashMaxValue);
        _dashMeter.enabled = !(_dashValue >= _dashMaxValue);
        _dashMeterBackground.enabled = !(_dashValue >= _dashMaxValue);


    }

    public void ResetDash()
    {
        _dashValue = 0;
        _playerIsDead = false;
    }

    public void SetDashGainedAfterEnemyKill(float value)
    {
        _dashAddedOnEnemyKill = value;
    }

    public void Dash()
    {

        if (_dashValue >= _dashMaxValue)
        {
            UnityAction dash = () => _levelManagerScript.ChangeSpeeds(_speedMultiplier);
            dash += () => _playerScript.SetDash(true);
            dash += () => _swipeDetectionScript.DisableInput();
            dash += () => AddDashValue(-Time.deltaTime);
            dash += () => _swipeDetectionScript.Tapped.RemoveListener(TapLeap);
            dash += () => _dashParticles.SetActive(true);

            UnityAction revertedSpeed = () => _levelManagerScript.RevertSpeeds();
            revertedSpeed += () => _playerScript.SetDash(false);
            revertedSpeed += () => _swipeDetectionScript.EnableInput();
            revertedSpeed += () => _swipeDetectionScript.Tapped.AddListener(TapLeap);
            revertedSpeed += () => _dashParticles.SetActive(false);

            StartCoroutine(TimerScript.Instance.CO_ExecuteDuringTimer(dash, _dashMaxValue, revertedSpeed));
        }
        
    }

    private void TapLeap()
    {
        if (_levelManagerScript.CheckEnemyCount() <= 0)
        {
            UnityAction leap = () => _levelManagerScript.ChangeSpeeds(_speedMultiplier);
            leap += () => _dashParticles.SetActive(true);

            UnityAction revertedSpeed = () => _levelManagerScript.RevertSpeeds();
            revertedSpeed += () => AddDashValue(_tapAddedValue);
            revertedSpeed += () => _dashParticles.SetActive(false);

            StartCoroutine(TimerScript.Instance.CO_ExecuteDuringTimer(leap, _tapSpeedTimer, revertedSpeed));
        }
    }

    private void LeapToKilledEnemy()
    {
        if (!_playerScript.IsDashing)
        {
            UnityAction leap = () => _levelManagerScript.ChangeSpeeds(_enemyDashSpeed);

            UnityAction revertedSpeed = () => _levelManagerScript.RevertSpeeds();

            UnityAction briefPause = () => _levelManagerScript.ChangeSpeeds(0f);
            UnityAction pauseSection = () => StartCoroutine(TimerScript.Instance.CO_ExecuteDuringTimer(briefPause, _enemyDashPauseTimer, revertedSpeed));

            StartCoroutine(TimerScript.Instance.CO_ExecuteDuringTimer(leap, _enemyDashSpeedTimer, pauseSection));
        }
    }


    private void AddDashValue(float value)
    {
        _dashValue += value;

        if (_dashValue > _dashMaxValue)
        {
            _dashValue = _dashMaxValue;
        }
    }
    private void OnEnemyKilled()
    {
        if (!_playerScript.IsDashing)
        {
            float value = _dashMaxValue * _dashAddedOnEnemyKill;
            AddDashValue(value);
            LeapToKilledEnemy();
        }
    }

    private void OnPlayerDamaged()
    {
        _playerIsDead = true;
    }


}
