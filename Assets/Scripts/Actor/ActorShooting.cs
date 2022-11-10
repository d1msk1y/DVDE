using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class ActorShooting : MonoBehaviour
{
    [Header("Essential references")]
    public Transform _firePos;
    public GameObject _gunHandler;
    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;
    private Joystick _shootingJoystick;

    [Header("Shooting properties")]
    public int gunIndex;
    public Gun defaultGun;
    public Gun gunScript;
    public Gun knife;

    [SerializeField]
    private float _gunThrowForce;
    [SerializeField]
    private float _bulletScaleModifier;
    [SerializeField]
    private float _gunThrowAimLength;

    public int criticalDamageChance;
    public int ammos;
    public int extraAmmos;
    public float extraAmmosPercent;
    [SerializeField] public float _damageOverride;
    public float damageModifier;
    public float aimLength;

    private int _damage;
    private float _recoilForce;
    private float _bulletSpeed;
    private float _rechargeSpeed;
    private float _bulletLifetime;

    private Color BulletColor => gunScript.bulletColorOverride != Color.white ? gunScript.bulletColorOverride : _spriteRenderer.color;

    [Header("Particles")]
    public ParticleSystem shotParticle;
    public LineRenderer _aimLineRenderer;

    [Header("Particles")]
    private float _currentRecharge;
    private Vector3 _handlerScale;
    private float _angle;
    private bool IsRecharged => _currentRecharge <= 0f;

    [HideInInspector]public Vector2 enemyPosition;

    private void Start() {
        defaultGun = GameManager.instance.itemsManager.buyableGuns[PlayerPrefs.GetInt("Picked gun")].gunProperties;
        GameManager.instance.itemsManager.purchasedWeapons = GameManager.instance.itemsManager.checkPurchasedGuns();
        gunScript = _gunHandler.GetComponentInChildren<Gun>();
        GiveWeapon(defaultGun);
        GameManager.instance.UiManager.SetAmmoStats(ammos, gunScript.ammos);
        _lineRenderer = GetComponent<LineRenderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _shootingJoystick = GameManager.instance.UiManager.shootingJoystick;
        
    }

    private void Update() {
        MouseAim();

        if (_currentRecharge > 0f) {
            _currentRecharge -= _rechargeSpeed * Time.deltaTime;
        }

        _gunHandler.transform.localScale = Vector3.Lerp(_gunHandler.transform.localScale, _handlerScale, 10f * Time.deltaTime);

        if (gunScript.weaponType == WeaponType.Knife) {
            return;
        }

        CheckHandler();
    }

    private void CheckHandler() {
        if (_gunHandler.transform.rotation.z > -0.7 && _gunHandler.transform.rotation.z < 0.7) {
            _handlerScale = new Vector3(1f, 1f, 1f);
            PlayerController.instance.clothSlotController.glassesSlot.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else {
            _handlerScale = new Vector3(1f, -1f, 1f);
            PlayerController.instance.clothSlotController.glassesSlot.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private Vector2 GetState() {
        if (PlayerController.instance.playerMovement.movementJoystick.Vertical == 0 || PlayerController.instance.playerMovement.movementJoystick.Horizontal == 0)
            return PlayerController.instance.playerMovement.movementJoystick.Direction;
        else
            return enemyPosition;
    }

    private void AutoAim() {
        if (GameManager.instance.lvlManager.lvlController != null
            && GameManager.instance.isCurrentBattle && GameManager.instance.lvlManager.lvlController.CurrentEnemiesInAction.Count != 0) {
            // enemyPosition = GetClosestEnemy(GameManager.instance.lvlManager.lvlController.CurrentEnemiesInAction).transform.position;//Mouse Position		

            enemyPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            enemyPosition -= (Vector2)transform.position;
            float angleGun = Mathf.Atan2(enemyPosition.y, enemyPosition.x) * Mathf.Rad2Deg;
            _gunHandler.transform.rotation = Quaternion.Euler(0, 0, angleGun);
            _angle = _gunHandler.transform.rotation.z;
        }
        else if (GameManager.instance.lvlManager.lvlController == null || !GameManager.instance.isCurrentBattle) {
            if (PlayerController.instance.playerMovement.movementJoystick.Vertical == 0 || PlayerController.instance.playerMovement.movementJoystick.Horizontal == 0)
                return;
            float z = Mathf.Atan2(PlayerController.instance.playerMovement.movementJoystick.Vertical, PlayerController.instance.playerMovement.movementJoystick.Horizontal) * Mathf.Rad2Deg;
            _gunHandler.transform.rotation = Quaternion.Euler(0, 0, z);
            _angle = _gunHandler.transform.eulerAngles.z;
        }
    }
    private void MouseAim() {

        enemyPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        enemyPosition -= (Vector2)transform.position;
        float angleGun = Mathf.Atan2(enemyPosition.y, enemyPosition.x)*Mathf.Rad2Deg;
        _gunHandler.transform.rotation = Quaternion.Euler(0, 0, angleGun);
        _angle = _gunHandler.transform.rotation.z;
    }

    private void JoyStickAim() {
        if (_shootingJoystick.Horizontal == 0f && _shootingJoystick.Vertical == 0f) return;
        var z = Mathf.Atan2(_shootingJoystick.Vertical, _shootingJoystick.Horizontal);
        _gunHandler.transform.rotation = Quaternion.Euler(0f, 0f, z);
        _aimLineRenderer.SetPosition(0, _firePos.position);
        _aimLineRenderer.SetPosition(1, _firePos.position + new Vector3(_shootingJoystick.Horizontal, _shootingJoystick.Vertical).normalized * aimLength);
        
        if (ammos > 0 || gunScript.gameObject.CompareTag("Knife")) return;
        _lineRenderer.SetPosition(0, _firePos.position);
        _lineRenderer.SetPosition(1, transform.position
            + new Vector3(_shootingJoystick.Horizontal, _shootingJoystick.Vertical)
            * _gunThrowAimLength);
    }

    private Transform GetClosestEnemy(List<GameObject> enemies) {
        Transform result = null;
        var num = float.PositiveInfinity;
        var position = transform.position;
        foreach (var gameObject in enemies) {
            if (gameObject == null) {
                return null;
            }
            var num2 = Vector3.Distance(gameObject.transform.position, position);
            if (num2 < num)
            {
                result = gameObject.transform;
                num = num2;
            }
        }
        return result;
    }

    public void GiveWeapon(Gun gun2Give)
    {
        if (gun2Give == gunScript)
        {
            return;
        }
        if(gunScript != null)Destroy(gunScript.gameObject);
        _gunHandler.transform.localScale = Vector3.one;
        var gameObjectInst = Instantiate(gun2Give.gameObject, _gunHandler.transform.position, Quaternion.identity, _gunHandler.transform);
        gunScript = (gameObjectInst.GetComponent(typeof(Gun)) as Gun);
        gunScript.transform.localEulerAngles = Vector3.zero;
        
        if (PlayerPrefs.HasKey(GameManager.instance.dataManager.specsKeys[3])) 
            extraAmmosPercent = PlayerPrefs.GetFloat(GameManager.instance.dataManager.specsKeys[3]);
        extraAmmos = (int)(gunScript.ammos * (extraAmmosPercent / 100f));
        SetGunProperties();
        GameManager.instance.UiManager.SetGunName(gun2Give.gunName);
        GameManager.instance.UiManager.SetAmmoStats(ammos, gunScript.ammos);
    }

    public void ThrowGun(GameObject gun2Throw)
    {
        if (gunScript == null || gunScript.CompareTag("Knife"))
            return;

        var instantiate = DropGun();
        instantiate.GetComponent<Rigidbody2D>().AddForce(_firePos.right*_gunThrowForce, ForceMode2D.Impulse);
        instantiate.GetComponent<Rigidbody2D>().angularVelocity = 1000f;

        Destroy(gunScript);
        GiveWeapon(knife);
        ClearAllLines();
    }
    private GameObject DropGun() {

        GameObject instantiate;
        if (ammos <= 0) {
            instantiate = Instantiate(gunScript.gun2Throw, _firePos.position, quaternion.identity);
            return instantiate;
        }
        instantiate = Instantiate(gunScript.gun2PickUp.gameObject, _firePos.position, Quaternion.identity);
        var gunProps = Instantiate(gunScript, Vector3.up * 1000, quaternion.identity, instantiate.transform);
        gunProps.ammos = ammos;
        instantiate.transform.rotation = new Quaternion(0, 0, 0, 0);
        instantiate.GetComponent<GunPickUp>().gunProperties = gunProps;
        return instantiate;
    }

    private void SetGunProperties()
    {
        gunIndex = gunScript.gunIndex;
        _recoilForce = gunScript.recoilForce;
        _bulletSpeed = gunScript.bulletSpeed;
        _rechargeSpeed = gunScript.rechargeSpeed;
        _bulletLifetime = gunScript.bulletLifetime;
        ammos = gunScript.ammos + extraAmmos;
        _damage = gunScript.damage;
        _firePos = gunScript.firePos;
    }

    public virtual void Shot(Rigidbody2D rb)
    {
        if (gunScript.weaponType == WeaponType.Knife || !IsRecharged)
        {
            return;
        }
        if (ammos <= 0)
        {
            SoundManager.PlayOneShot(gunScript.noAmmoSound);
            _currentRecharge = 1f;
            return;
        }
        //gunScript.audioSource.pitch = Random.Range(0.6f, 1.5f);
        SoundManager.PlayOneShot(gunScript.shotSound);
        GameManager.instance.ShakeOnce(gunScript.shakeForce);
        rb.AddForce(-_firePos.right * _recoilForce, ForceMode2D.Impulse);
        SpawnBullet();
        _currentRecharge = 1f;
        if (GameManager.instance.isGameStarted) 
            ammos--;
        GameManager.instance.UiManager.SetAmmoStats(ammos, gunScript.ammos + extraAmmos);
    }

    private void SpawnBullet()
    {
        GameObject gameObject = Instantiate(gunScript.bullet, _firePos.position, _gunHandler.transform.rotation);
        if (gunScript.weaponType == WeaponType.Shotgun)
        {
            Bullet[] componentsInChildren = gameObject.GetComponentsInChildren<Bullet>();
            gameObject.transform.localScale *= _bulletScaleModifier;
            foreach (Bullet bulletProperties in componentsInChildren)
            {
                SetBulletProperties(bulletProperties);
            }
            return;
        }
        SetBulletProperties(gameObject.GetComponent<Bullet>());
    }

    public void ClearAllLines()
    {
        ClearLineRenderer(_lineRenderer);
        ClearLineRenderer(_aimLineRenderer);
    }

    private void ClearLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
    }

    private void SetBulletProperties(Bullet curBullet)
    {
        var num = gunScript.damage * damageModifier * _damageOverride;
        _damage = (int)num;
        curBullet.damage = _damage;
        curBullet.bulletSpeed = _bulletSpeed;
        curBullet.shootingScript = this;
        curBullet.parent = gameObject;
        curBullet.lifeTime = _bulletLifetime;
        curBullet.criticalDamageChance = criticalDamageChance;
        curBullet.trailRenderer.startWidth *= _bulletScaleModifier;
        SetBulletColor(curBullet);
        Instantiate(shotParticle, _firePos.position, Quaternion.identity);
    }

    private void SetBulletColor(Bullet bulletObj)
    {
        bulletObj.spriteRenderer.color = BulletColor;
        bulletObj.trailRenderer.material.color = BulletColor;
    }

}