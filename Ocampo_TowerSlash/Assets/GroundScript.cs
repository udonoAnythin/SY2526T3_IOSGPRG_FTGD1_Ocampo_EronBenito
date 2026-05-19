using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Material _groundMaterial;

    private void Update()
    {
        ScrollGround();
    }
    private void ScrollGround()
    {
        _groundMaterial.mainTextureOffset += Vector2.right * _speed * Time.deltaTime;
    }
}
