using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    Pistol = 0,
    AutomaticRifle = 1,
    Shotgun = 2
}

public enum GunMode
{
    Semi_Automatic,
    Automatic
}

public enum GunState
{
    Idle,
    Firing,
    Reloading
}

[CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    public GunType Type
    { get => _gunType; }

    public GunMode Mode
    { get => _gunMode; }

    public Sprite GunWorldSprite
    { get => _gunWorldSprite; }

    public Sprite GunHoldSprite
    { get => _gunHoldSprite; }

    public int MagSize
    { get => _magSize; }

    public int Damage
    { get => _damage; }

    public float FireRate
    { get => _fireRate; }

    public float ReloadSpeed
    { get => _reloadSpeed; }

    public float ShotgunArcAngle
    { get => _gunType == GunType.Shotgun ? _shotgunArcAngle : 0; }

    [Header("Loaded Bullet Data")]
    public int currentLoadedBullets;
    [SerializeField] private int _magSize;

    [Header("Gun Data")]
    [SerializeField] private GunType _gunType;
    [SerializeField] private GunMode _gunMode;
    [SerializeField] private Sprite _gunWorldSprite;
    [SerializeField] private Sprite _gunHoldSprite;
    [SerializeField] private int _damage;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _reloadSpeed;

    [SerializeField] private float _shotgunArcAngle;
}
