using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManagerScript : MonoBehaviour
{

    [SerializeField] private AudioClip _bgm;

    [Header("Affected GameObjects")]
    [SerializeField] private SpawnerScript _spawner;
    [SerializeField] private GroundScript _ground;
    [SerializeField] private PlayerScript _player;
    [SerializeField] private SwipeDetectionScript _swipeInputs;
    [SerializeField] private DashScript _dashScript;

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

    private void Start()
    {
        Pause();

        
    }

    public void Pause()
    {
        _swipeInputs.DisableInput();
        _spawner.StopSpawningEnemies();
        ChangeSpeeds(0);
    }

    public void Play()
    {
        _swipeInputs.EnableInput();
        _spawner.StartSpawningEnemies();

        SoundManagerScript.Instance.PlayBGM(_bgm);

        ChangeSpeeds(1);
    }

    public void Retry()
    {
        _swipeInputs.EnableInput();
        _gameOverObject.SetActive(false);
        _spawner.ResetAllEnemies();
        _spawner.StartSpawningEnemies();
        _dashScript.ResetDash();
        _player.SetSpriteEnabled(true);

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
        _livesText.text = "Lives Left: " + _player.Lives;
        _player.SetSpriteEnabled(false);

        if (_player.Lives <= 0)
        {
            _retryButton.gameObject.SetActive(false);
        }

        _spawner.StopArrowChanges();
        ChangeSpeeds(0f);
        _spawner.StopSpawningEnemies();
    }



}
