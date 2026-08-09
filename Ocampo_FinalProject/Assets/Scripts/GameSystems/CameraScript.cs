using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraScript : MonoBehaviour
{
    
    public static CameraScript instance;

    [SerializeField] private Camera _camera;

    private Vector3 _initialLocalCameraPosition;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _initialLocalCameraPosition = transform.localPosition;
        instance = this;
    }

    public void ShakeCamera(float camShakeTimer, float camShakeIntensity)
    {
        UnityAction cameraShake = () =>
        {
            float randomX = Random.Range(-camShakeIntensity, camShakeIntensity);
            float randomY = Random.Range(-camShakeIntensity, camShakeIntensity);

            _camera.transform.localPosition = _initialLocalCameraPosition + new Vector3(randomX, randomY, _initialLocalCameraPosition.z);
        };

        UnityAction restoreCamera = () =>
        {
            _camera.transform.localPosition = _initialLocalCameraPosition;
        };

        TimerScript.instance.StartCoroutine(TimerScript.instance.CO_ExecuteDuringTimer(camShakeTimer, cameraShake, restoreCamera));
    }

}
