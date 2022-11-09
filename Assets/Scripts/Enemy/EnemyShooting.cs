using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Essential references")]
    public EnemyController enemyController;
    public Transform firePos;
    public GameObject gunObj;

    [Header("Shooting properties")]
    public float recoilForceModifier;
    public float bulletSpeedModifier;
    public float rechargeSpeedModifier;
    public float damageModifier;
    public float rechargeSpeed;
    public float bulletSpeed;
    public float _bulletLifetime;
    public int damage;
    public GameObject gun;
    public Gun gunScript;

    [Header("Particles")]
    public ParticleSystem shotParticle;

    private float _recharge;
    [HideInInspector]public bool isRecharged = true;
    private Vector3 _handlerScale;
    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;

    private void OnEnable()
    {
        randomizeGun();

        _lineRenderer = GetComponent<LineRenderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void randomizeGun()
    {
        int randWeapon = Random.Range(0, GameManager.instance.itemsManager.purchasedWeapons.Count);
        if (randWeapon > GameManager.instance.itemsManager.purchasedWeapons.Count)
        {
            randWeapon = GameManager.instance.itemsManager.purchasedWeapons.Count;
        }
        GiveWeapon(GameManager.instance.itemsManager.purchasedWeapons[randWeapon]);
    }

    public void GiveWeapon(Gun gun2Give)
    {
        if (gun2Give == gunScript)
            return;
        Destroy(gun);
        gun = Instantiate(gun2Give.gameObject, gunObj.transform.position, Quaternion.identity);
        gunScript = gun.GetComponent(typeof(Gun)) as Gun;
        gun.transform.parent = gunObj.transform;
        firePos = gunScript.firePos;
        SetGunProperties();

    }

    private void SetGunProperties()
    {
        bulletSpeed = gunScript.bulletSpeed * bulletSpeedModifier;
        rechargeSpeed = gunScript.rechargeSpeed * rechargeSpeedModifier;
        _bulletLifetime = gunScript.bulletLifetime;
        float damages = (float)gunScript.damage * damageModifier;
        damage = (int) damages;
        firePos = gunScript.firePos;

    }
        // Update is called once per frame
    private void Update()
    {
        //---Recharge timer---//
        if (_recharge > 0)
            _recharge -= rechargeSpeed * Time.deltaTime;
        if (_recharge <= 0)
            isRecharged = true;
        //---Recharge timer---//

        //---Scoping---//
        if (!PlayerController.instance.isAlive)
            return;
        
            Vector3 lookPos = PlayerController.instance.transform.position;//Get a player's vector3 variable.
            lookPos -= transform.position;//Calculate the direction from enemy to player.
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;//Geting needed gun rotation angle.
            gunObj.transform.rotation = Quaternion.Euler(0, 0, angle);//Set the gun rotation.
        //---Scoping---//        

        gunObj.transform.localScale = Vector3.Lerp(gunObj.transform.localScale, _handlerScale, 0.3f);

        if (angle > -90 && angle < 90)
        {
            _handlerScale = new Vector3(1, 1, 1);
        }
        else
            _handlerScale = new Vector3(1, -1, 1);

        gunObj.transform.rotation = Quaternion.Euler(0, 0, angle);

    }

    public void Shot()
    {
        if (!isRecharged)
            return;//If gun isnt recharged - return.
        isRecharged = false;
        _recharge = 1;//recharge timer reset.
        
        SoundManager.PlayOneShot(gunScript.shotSound);

        SpawnBullet();
    }
    private void SpawnBullet()
    {
        GameObject curBullet = Instantiate(gunScript.bullet, firePos.position, gunObj.transform.rotation) as GameObject;
        if (gunScript.weaponType == WeaponType.Shotgun)
        {
            Bullet[] fractionedBullets = curBullet.GetComponentsInChildren<Bullet>();
            foreach (Bullet bullet in fractionedBullets)
            {
                SetBulletProperties(bullet);
            }
            return;
        }

        SetBulletProperties(curBullet.GetComponent<Bullet>());

    }

    private void SetBulletProperties(Bullet curBullet)
    {
        Bullet bulletComponent = curBullet.GetComponent<Bullet>();//Geting a bullet component.
        bulletComponent.damage = damage;//Set bullet's damage.
        bulletComponent.parent = gameObject;//Set bullet's parent.
        bulletComponent.bulletSpeed = bulletSpeed;//Set bullet's speed.
        bulletComponent.lifeTime = _bulletLifetime;
        SetBulletColor(bulletComponent);
        Instantiate(shotParticle, firePos.position, Quaternion.identity);//Particle instantiate.
    }

    private void SetBulletColor(Bullet bullet_obj)
    {
        bullet_obj.spriteRenderer.color = _spriteRenderer.color;
        bullet_obj.trailRenderer.material.color = _spriteRenderer.color;
    }
}