using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManagerScript : MonoBehaviour
{
    
    public static GameManagerScript instance;

    [Header("Entity References")]
    [SerializeField] private List<EnemyStatsScript> _enemies;
    [SerializeField] private PlayerStatsScript _player;

    [Header("Results UI References")]
    [SerializeField] private TextMeshProUGUI _enemiesLeftCounter;
    [SerializeField] private GameObject _gameplayUI;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private List<ParticleSystem> _victoryParticles;

    [Header("Kill Feed Reference")]
    [SerializeField] private GameObject _killFeedPrefab;
    [SerializeField] private Transform _killFeedParent;

    private void Awake()
    {
        instance = this;  
    }

    private void Update()
    {
        _enemiesLeftCounter.text = _enemies.Count.ToString();
    }

    public void AddEnemy(EnemyStatsScript enemy)
    {
        _enemies.Add(enemy);
        enemy.onDeath.AddListener(ShowKillfeed);
        enemy.onDeath.AddListener(DeclarePlayerWin);

        _player.onDeath.AddListener(EndGame);

    }

    private void ShowKillfeed(EntityStatsScript victim, EntityStatsScript killer)
    {
        // Instantiate Kill feed
        GameObject newKillfeed = Instantiate(_killFeedPrefab, _killFeedParent);

        // Set the child to be the first transform of the killfeed parent
        newKillfeed.transform.SetAsFirstSibling();

        // Rewrite the feed
        TextMeshProUGUI killFeedText = newKillfeed.GetComponentInChildren<TextMeshProUGUI>();
        killFeedText.text = $"{killer.entityName} has killed {victim.entityName}";

        // Set Fade Transitions
        UnityAction killfeedFadeOut = () => 
        { 
            CanvasGroup killfeedCanvasGroup = newKillfeed.GetComponent<CanvasGroup>();
            killfeedCanvasGroup.alpha = Mathf.Lerp(killfeedCanvasGroup.alpha, 0, 0.05f);
        };

        UnityAction killFeedDestroy = () => Destroy(newKillfeed);

        UnityAction killFeedTransition = () => StartCoroutine(TimerScript.instance.CO_ExecuteDuringTimer(1, killfeedFadeOut, killFeedDestroy));

        StartCoroutine(TimerScript.instance.CO_ExecuteAfterTimer(4, killFeedTransition));

        // Remove enemy from list
        _enemies.Remove(victim as EnemyStatsScript);
    }

    private void EndGame(EntityStatsScript victim, EntityStatsScript killer)
    {
        if (_enemies.Count > 0)
        {
            _gameplayUI.SetActive(false);
            _gameOverScreen.SetActive(true);
        }
    }

    private void DeclarePlayerWin(EntityStatsScript victim, EntityStatsScript killer)
    {
        // If there are no more enemies and the player is still alive
        if (_enemies.Count == 0 && _player != null)
        {
            _gameplayUI.SetActive(false);
            _victoryScreen.SetActive(true);
        }
    }

}
