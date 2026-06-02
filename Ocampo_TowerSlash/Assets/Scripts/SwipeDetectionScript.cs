using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum SwipeDirection
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3
}

public class SwipeDetectionScript : MonoBehaviour
{

    public UnityEvent SwipedLeft
    {
        get => _swipedLeft;
    }

    public UnityEvent SwipedRight
    {
        get => _swipedRight;
    }

    public UnityEvent SwipedUp
    {
        get => _swipedUp;
    }

    public UnityEvent SwipedDown
    {
        get => _swipedDown;
    }

    public UnityEvent Tapped
    {
        get => _tapped;
    }

    [SerializeField] private PlayerInput _playerInput;

    [Header("Event References")]
    private UnityEvent _swipedLeft = new UnityEvent();
    private UnityEvent _swipedRight = new UnityEvent();
    private UnityEvent _swipedUp = new UnityEvent();
    private UnityEvent _swipedDown = new UnityEvent();
    private UnityEvent _tapped = new UnityEvent();

    private InputAction _touchPressAction;
    private InputAction _touchPositionAction;

    private Vector2 _touchStart;// = Vector2.zero;
    private Vector2 _touchEnd;// = Vector2.zero;

    private void Awake()
    {
        _touchPositionAction = _playerInput.actions["TouchPosition"];
        _touchPressAction = _playerInput.actions["TouchPress"];
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

    public void EnableInput()
    {
        _playerInput.actions.Enable();
    }

    public void DisableInput()
    {
        _playerInput.actions.Disable();
    }

    public bool CheckIfInputEnabled()
    {
        return _playerInput.actions.enabled;
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        _touchStart = _touchPositionAction.ReadValue<Vector2>();
        //Debug.Log($"Touch Start {_touchStart}");
        //Debug.Log("Tapped!");
    }

    private void OnTouchReleased(InputAction.CallbackContext context)
    {
        _touchEnd = _touchPositionAction.ReadValue<Vector2>();
        
        Vector2 direction = _touchEnd - _touchStart;

        //Actions when tapped
        if (direction.magnitude < 1f)
        {
            //Tapped Functions
            Debug.Log("Tapped");

            _tapped.Invoke();

            return;
        }
        else
        {
            direction.Normalize();
        }

        //Debug.Log($"Touch Start {_touchStart}");
        //Debug.Log($"Touch End: {_touchEnd}");

        //Left Direction Swipe
        if (Vector2.Dot(direction, Vector2.left) > 0.5f)
        {
            Debug.Log("Player Swiped Left");
            _swipedLeft.Invoke();
        }

        //right Direction Swipe
        else if (Vector2.Dot(direction, Vector2.right) > 0.5f)
        {
            Debug.Log("Player Swiped Right");
            _swipedRight.Invoke();
        }

        //up Direction Swipe
        else if (Vector2.Dot(direction, Vector2.up) > 0.5f)
        {
            Debug.Log("Player Swiped Up");
            _swipedUp.Invoke();
        }

        //down Direction Swipe
        else if (Vector2.Dot(direction, Vector2.down) > 0.5f)
        {
            Debug.Log("Player Swiped Down");
            _swipedDown.Invoke();
        }

    }



}
