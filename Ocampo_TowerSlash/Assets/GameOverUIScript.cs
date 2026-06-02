using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameOverUIScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeInTime = 0.75f;
    [SerializeField] private float _fadeInSpeed = 0.1f;

    private void OnEnable()
    {
        UnityAction fadeIn = () => _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1, _fadeInSpeed);

        StartCoroutine(TimerScript.Instance.CO_ExecuteDuringTimer(fadeIn, _fadeInTime));
    }

    private void OnDisable()
    {
        _canvasGroup.alpha = 0;
    }
}
