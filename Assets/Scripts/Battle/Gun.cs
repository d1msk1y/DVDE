using FMODUnity;
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

    public Gun(Gun gunProperties) {
        gunIndex = gunProperties.gunIndex;
        weaponType = gunProperties.weaponType;
        shotSound = gunProperties.shotSound;
        noAmmoSound = gunProperties.noAmmoSound;
        gunPickUpSound = gunProperties.gunPickUpSound;
        firePos = gunProperties.firePos;
        gun2Throw = gunProperties.gun2Throw;
        gun2PickUp = gunProperties.gun2PickUp;
        bullet = gunProperties.bullet;
        bulletColorOverride = gunProperties.bulletColorOverride;
        gunName = gunProperties.gunName;
        recoilForce = gunProperties.recoilForce;
        bulletSpeed = gunProperties.bulletSpeed;
        rechargeSpeed = gunProperties.rechargeSpeed;
        bulletLifetime = gunProperties.bulletLifetime;
        shakeForce = gunProperties.shakeForce;
        damage = gunProperties.damage;
        pushForce = gunProperties.pushForce;
        ammos = gunProperties.ammos;
    }
}
