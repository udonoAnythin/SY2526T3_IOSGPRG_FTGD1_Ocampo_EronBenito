using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraScript : MonoBehaviour
{

    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private float _cameraShakeIntensity;
    [SerializeField] private float _cameraShakeDuration;

    private Vector3 _position;
    private Vector3 _offset = Vector3.zero;

    private void Start()
    {
        _position = transform.position;
    }

    private void OnEnable()
    {
        _playerScript.EnemyKilled.AddListener(CameraShake);
    }

    private void OnDisable()
    {
        _playerScript.EnemyKilled.RemoveListener(CameraShake);
    }

    private void Update()
    {
        transform.position = _position + _offset;
    }

    private void CameraShake()
    {
        UnityAction setOffset = () => _offset = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0) * _cameraShakeIntensity;
        //setOffset += () => Debug.Log($"Set Offset : {_offset}");

        UnityAction resetOffset = () => _offset = Vector3.zero;
        //resetOffset += () => Debug.Log("Reset Offset");

        StartCoroutine(TimerScript.Instance.CO_ExecuteDuringTimer(setOffset, _cameraShakeDuration, resetOffset));

        
    }

}
