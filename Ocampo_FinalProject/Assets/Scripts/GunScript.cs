using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    Pistol = 0,
    AutomaticRifle = 1,
    Shotgun = 2
}

public class GunScript : MonoBehaviour
{

    public GunType Type
    { get => _gunType; }

    public int magSize;
    public int currentLoadedBullets;

    private GunType _gunType;

}
