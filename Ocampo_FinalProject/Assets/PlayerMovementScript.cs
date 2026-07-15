using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{

    [Header("Input")]
    [SerializeField] private PlayerControlScript _controlScript;

    [Header("Movement Values")]
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _acceleration = 0.5f;

    [Header("Aim Values")]
    [SerializeField] private GameObject _body;
    [SerializeField] private float _aimAcceleration = 0.5f;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _controlScript = GetComponent<PlayerControlScript>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        DoAim();
    }

    private void FixedUpdate()
    {
        DoMovement();
    }

    private void DoMovement()
    {
        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, _controlScript.MovementVector * _moveSpeed, _acceleration);
    }

    private void DoAim()
    {
        _body.transform.rotation = Quaternion.Lerp(_body.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(_controlScript.AimVector.y, _controlScript.AimVector.x) * Mathf.Rad2Deg), _aimAcceleration);
    }


}
