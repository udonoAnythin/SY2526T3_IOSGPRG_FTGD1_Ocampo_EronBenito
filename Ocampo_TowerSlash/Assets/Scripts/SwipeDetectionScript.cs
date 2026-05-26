using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SwipeDirection
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3
}

public class SwipeDetectionScript : MonoBehaviour
{

    [SerializeField] private PlayerInput _playerInput;

    [Header("GameObject References")]
    [SerializeField] private PlayerScript _playerScript;
    [SerializeField] private GroundScript _groundScript;

    private InputAction _touchPressAction;
    private InputAction _touchPositionAction;

    private Vector2 _touchStart;
    private Vector2 _touchEnd;

    private void Awake()
    {
        _touchPressAction = _playerInput.actions["TouchPress"];
        _touchPositionAction = _playerInput.actions["TouchPosition"];
    }

    private void OnEnable()
    {
        _touchPressAction.started += OnTouchStarted;
        _touchPressAction.canceled += OnTouchReleased;
    }

    private void OnDisable()
    {
        _touchPressAction.started -= OnTouchStarted;
        _touchPressAction.canceled -= OnTouchReleased;
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        _touchStart = _touchPositionAction.ReadValue<Vector2>();
    }

    private void OnTouchReleased(InputAction.CallbackContext context)
    {
        _touchEnd = _touchPositionAction.ReadValue<Vector2>();
        
        Vector2 direction = _touchEnd - _touchStart;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (_touchEnd.x < _touchStart.x)
            {
                Debug.Log("Player Swiped Left");
                _playerScript.AttackNearestEnemy(SwipeDirection.Left);

            }
            else if (_touchEnd.x > _touchStart.x)
            {
                Debug.Log("Player Swiped Right");
                _playerScript.AttackNearestEnemy(SwipeDirection.Right);

            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (_touchEnd.y < _touchStart.y)
            {
                Debug.Log("Player Swiped Down");
                _playerScript.AttackNearestEnemy(SwipeDirection.Down);
            }
            else if (_touchEnd.y > _touchStart.y)
            {
                Debug.Log("Player Swiped Up");
                _playerScript.AttackNearestEnemy(SwipeDirection.Up);

            }
        }

    }



}
