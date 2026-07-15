using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlScript : MonoBehaviour
{

    public Vector2 MovementVector
    { get => _movementJoystick.Direction; }

    public Vector2 AimVector
    { get => _aimJoystick.Direction; }

    [Header("Input")]
    [SerializeField] private Joystick _movementJoystick;
    [SerializeField] private Joystick _aimJoystick;

}
