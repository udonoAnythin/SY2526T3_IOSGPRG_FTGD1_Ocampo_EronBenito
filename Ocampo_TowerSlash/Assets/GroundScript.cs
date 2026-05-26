using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    [SerializeField] private float _baseSpeed;
    [SerializeField] private Material _groundMaterial;
    
    private float _speed;

    private void Start()
    {
        _speed = _baseSpeed;
    }

    private void Update()
    {
        ScrollGround();
    }
    private void ScrollGround()
    {
        _groundMaterial.mainTextureOffset += Vector2.right * _speed * Time.deltaTime;
    }

    private void ChangeSpeed(float multiplier)
    {
        _speed = _baseSpeed * multiplier;
    }

    private void RevertSpeed()
    {
        _speed = _baseSpeed;
    }
}
