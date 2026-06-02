using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterSelectScript : MonoBehaviour
{
    [Header("Affected GameObjects")]
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private DashScript _dashScript;
    [SerializeField] private LevelManagerScript _levelManagerScript;

    [Header("Variables")]
    [SerializeField] private Animator _animator;
    [SerializeField] private int _playerTankHP = 5;
    [SerializeField] private float _dashPerEnemyKillPercent = 0.1f;

    private void Start()
    {
        
    }

    public void SetDefault()
    {
        _playerScript.SetCharacter(Character.Default);
        StartGame();
    }

    public void SetTank()
    {
        _playerScript.SetCharacter(Character.Tank);
        _playerScript.Lives = _playerTankHP;
        StartGame();
    }

    public void SetSpeedster()
    {
        _playerScript.SetCharacter(Character.Speed);
        _dashScript.SetDashGainedAfterEnemyKill(_dashPerEnemyKillPercent);
        StartGame();
    }

    private void StartGame()
    {
        _playerScript.GetComponent<SpriteRenderer>().enabled = true;
        _animator.Play("CharacterSelectExit");

        UnityAction play = () => _levelManagerScript.Play();
        play += () => gameObject.SetActive(false);

        StartCoroutine(TimerScript.Instance.CO_ExecuteInCountdown(play, 1f));
    }

}
