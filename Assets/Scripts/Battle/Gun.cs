using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public enum WeaponType
{
    Pistol,
    Riffle,
    LaserGun,
    BulletThrower,
    Shotgun,
    Knife
}

public class Gun : MonoBehaviour
{
    public int gunIndex;

    public WeaponType weaponType;

    [Header("Audio")]
    public EventReference shotSound;
    public EventReference noAmmoSound;
    public EventReference gunPickUpSound;

    [Header("References")]
    public Transform firePos;
    public GameObject gun2Throw;
    public GunPickUp gun2PickUp;
    public GameObject bullet;

    [Header("Gun properties")]
    public Color bulletColorOverride = Color.white;
    public string gunName;
    public float recoilForce;
    public float bulletSpeed;
    public float rechargeSpeed;
    public float bulletLifetime;
    public Vector4 shakeForce;
    public int damage;
    public float pushForce;
    public int ammos;
}
