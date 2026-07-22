using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    public GunType Type
    { get => _gunType; }

    public Sprite GunWorldSprite
    { get => _gunWorldSprite; }

    public Sprite GunHoldSprite
    { get => _gunHoldSprite; }

    public int MagSize
    { get => _magSize; }

    [Header("Loaded Bullet Data")]
    public int currentLoadedBullets;
    [SerializeField] private int _magSize;

    [Header("Gun Data")]
    [SerializeField] private GunType _gunType;
    [SerializeField] private Sprite _gunWorldSprite;
    [SerializeField] private Sprite _gunHoldSprite;
}
