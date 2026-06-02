using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    [Header("GameObject References")]
    [SerializeField] private PlayerScript _player;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private float _score = 0;

    private void Start()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();
        _scoreText.text = "Score: " + _score.ToString();
    }

    private void OnEnable()
    {
        _player.EnemyKilled.AddListener(OnEnemyKilled);
    }

    private void OnDisable()
    {
        _player.EnemyKilled.RemoveListener(OnEnemyKilled);
    }

    private void OnEnemyKilled()
    {
        _score++;
        _scoreText.text = "Score: " + _score.ToString();
    }
}
