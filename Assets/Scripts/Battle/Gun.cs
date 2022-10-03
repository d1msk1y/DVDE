using System.Collections;
using System.Collections.Generic;
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
    public AudioSource audioSource;
    public AudioClip shotSound;
    public AudioClip noAmmoSound;
    public AudioClip gunPickUpSound;

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

    private void Start()
    {
        if (weaponType == WeaponType.Knife)
            return;
        audioSource = GetComponent<AudioSource>();
        switch (weaponType)
        {
            case WeaponType.Shotgun:
                audioSource.volume = GameManager.instance.soundManager.sfxVolume + 0.1f;
                break;
            case WeaponType.BulletThrower:
                audioSource.volume = GameManager.instance.soundManager.sfxVolume + 0.15f;
                break;
        }
    }
}
