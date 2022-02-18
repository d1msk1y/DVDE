using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Essential references")]
    public Transform firePos;
    public GameObject gunObj;
    public GameObject bullet;

    [Header("Shooting properties")]
    public int gunIndex;
    public GameObject gun;
    public Gun gunScript;
    [HideInInspector]public int ammos;
    private int _damage;
    private float _recoilForce;
    private float _bulletSpeed;
    private float _rechargeSpeed;
    private float _bulletLifetime;

    [Header("Particles")]
    public ParticleSystem shotParticle;


    private float _currentRecharge;
    bool _isRecharged = true;
    private Vector3 _handlerScale;

    private void Start()
    {
        gun = gunObj.GetComponentInChildren<Gun>().gameObject;
        GiveWeapon(GetComponentInChildren<Gun>());
    }

    public void GiveWeapon(Gun gun2Give)
    {
        if (gun2Give == gunScript)
            return;
        Destroy(gun);
        gunObj.transform.localScale = Vector3.one;
        gun = Instantiate(gun2Give.gameObject, gunObj.transform.position, Quaternion.identity, gunObj.transform);
        gunScript = gun.GetComponent(typeof(Gun)) as Gun;        
        SetGunProperties();
        
    }

    private void SetGunProperties()
    {
        gunIndex = gunScript.gunIndex;
        _recoilForce = gunScript.recoilForce;
        _bulletSpeed = gunScript.bulletSpeed;
        _rechargeSpeed = gunScript.rechargeSpeed;
        _bulletLifetime = gunScript.bulletLifetime;
        ammos = gunScript.ammos;
        _damage = gunScript.damage;
        firePos = gunScript.firePos;
    }

    private void Update()
    {
        if(_currentRecharge > 0)
            _currentRecharge -= _rechargeSpeed * Time.deltaTime;
        if (_currentRecharge <= 0)
            _isRecharged = true;

        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);//Mouse/Touch Position
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(screenPosition);//End point of a ray
        lookPos -= transform.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;//Geting needed gun rotation angle 

        gunObj.transform.localScale = Vector3.Lerp(gunObj.transform.localScale, _handlerScale,0.3f);

        if (angle > -90 && angle < 90)
        {
            _handlerScale = new Vector3(1, 1, 1);
        }
        else
            _handlerScale = new Vector3(1, -1, 1);

        gunObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Shot(Rigidbody2D rb)
    {

        if (!_isRecharged || ammos <= 0)
        {
            return;
        }

        GameManager.instance.ShakeOnce();

        rb.AddForce(-firePos.right * _recoilForce, ForceMode2D.Impulse);

        GameObject curBullet = Instantiate(bullet, firePos.position, gunObj.transform.rotation) as GameObject;
        Bullet bulletComponent = curBullet.GetComponent<Bullet>();
        bulletComponent.damage = _damage;
        bulletComponent.bulletSpeed = _bulletSpeed;
        bulletComponent.parent = gameObject;
        bulletComponent.lifeTime = _bulletLifetime;
        
        Instantiate(shotParticle, firePos.position, Quaternion.identity);
        //Bullet.instance.rb.AddForce(firePos.right * bulletSpeed, ForceMode2D.Impulse);
        _isRecharged = false;
        _currentRecharge = 1;
        ammos -= 1;
    }

}
