using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManagerScript : MonoBehaviour
{

    [Header("Affected GameObjects")]
    [SerializeField] private SpawnerScript _spawner;
    [SerializeField] private GroundScript _ground;
    [SerializeField] private PlayerScript _player;
    [SerializeField] private SwipeDetectionScript _swipeInputs;

    [Header("UI Elements")]
    [SerializeField] private GameObject _gameOverObject;
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private Button _retryButton;

    private void OnEnable()
    {
        _player.PlayerDamaged.AddListener(OnPlayerDamaged);
    }

    private void OnDisable()
    {
        _player.PlayerDamaged.RemoveListener(OnPlayerDamaged);
    }

    public void Retry()
    {
        _swipeInputs.EnableInput();
        _gameOverObject.SetActive(false);
        _spawner.ResetAllEnemies();
        _spawner.StartSpawningEnemies();

        ChangeSpeeds(1f);
    }

    public void ChangeSpeeds(float multiplier)
    {
        _spawner.ChangeEnemySpeed(multiplier);
        _ground.ChangeSpeed(multiplier);
    }

    public void RevertSpeeds()
    {
        _spawner.RevertEnemySpeed();
        _ground.RevertSpeed();
    }

    public int CheckEnemyCount()
    {
        return _spawner.CheckEnemyCount();
    }

    private void OnPlayerDamaged()
    {
        _swipeInputs.DisableInput();
        _gameOverObject.SetActive(true);
        _livesText.text = "Lives: " + _player.Lives;
        

        if (_player.Lives <= 0)
        {
            _retryButton.gameObject.SetActive(false);
        }

        ChangeSpeeds(0f);
        _spawner.StopSpawningEnemies();
    }



}
