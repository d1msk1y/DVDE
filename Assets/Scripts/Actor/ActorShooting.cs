using System;
using UnityEngine;

public class ActorShooting : MonoBehaviour
{
	[Header("Essential references")]
	public Transform _firePos;
	public GameObject _gunObj;
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
	private int _damage;
	public float extraAmmosPercent;
	public float damageModifier;
	public float aimLength;
	private float _recoilForce;

	private float _bulletSpeed;

	private float _rechargeSpeed;

	private float _bulletLifetime;

	[Header("Particles")]
	public ParticleSystem shotParticle;

	public LineRenderer _aimLineRenderer;

	[Header("Particles")]
	private float _currentRecharge;

	private Vector3 _handlerScale;

	private float angle;

	private bool _isRecharged = true;

	private void Start()
	{
		defaultGun = GameManager.instance.itemsManager.buyableGuns[PlayerPrefs.GetInt("Picked gun")].gunProperties;
		GameManager.instance.itemsManager.purchasedWeapons = GameManager.instance.itemsManager.checkPurchasedGuns();
		gunScript = _gunObj.GetComponentInChildren<Gun>();
		GiveWeapon(defaultGun);
		GameManager.instance.UiManager.SetAmmoStats(ammos, gunScript.ammos);
		_lineRenderer = base.GetComponent<LineRenderer>();
		_spriteRenderer = base.GetComponent<SpriteRenderer>();
		_shootingJoystick = GameManager.instance.UiManager.shootingJoystick;
	}

	private void Update()
	{
		if (_currentRecharge > 0f)
		{
			_currentRecharge -= _rechargeSpeed * Time.deltaTime;
		}
		if (_currentRecharge <= 0f)
		{
			_isRecharged = true;
		}
		if (_shootingJoystick.Horizontal != 0f || _shootingJoystick.Vertical != 0f)
		{
			float z = Mathf.Atan2(_shootingJoystick.Vertical, _shootingJoystick.Horizontal) * 57.29578f;
			_gunObj.transform.rotation = Quaternion.Euler(0f, 0f, z);
			_aimLineRenderer.SetPosition(0, _firePos.position);
			_aimLineRenderer.SetPosition(1, _firePos.position + new Vector3(_shootingJoystick.Horizontal, _shootingJoystick.Vertical).normalized * aimLength);
			if (ammos <= 0 && !gunScript.gameObject.CompareTag("Knife"))
			{
				_lineRenderer.SetPosition(0, _firePos.position);
				_lineRenderer.SetPosition(1, base.transform.position + new Vector3(_shootingJoystick.Horizontal, _shootingJoystick.Vertical) * _gunThrowAimLength);
			}
		}
		_gunObj.transform.localScale = Vector3.Lerp(_gunObj.transform.localScale, _handlerScale, 10f * Time.deltaTime);
		if (gunScript.weaponType == WeaponType.Knife)
		{
			return;
		}
		if ((angle > -90f && angle < 90f) || _shootingJoystick.Horizontal > 0f)
		{
			_handlerScale = new Vector3(1f, 1f, 1f);
			PlayerController.instance.clothSlotController.glassesSlot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			_handlerScale = new Vector3(1f, -1f, 1f);
			PlayerController.instance.clothSlotController.glassesSlot.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		if (_shootingJoystick.Horizontal < 0f)
		{
			_handlerScale = new Vector3(1f, -1f, 1f);
			PlayerController.instance.clothSlotController.glassesSlot.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
	}

	private Transform GetClosestEnemy(GameObject[] enemies)
	{
		Transform result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		foreach (GameObject gameObject in enemies)
		{
			if (gameObject == null)
			{
				return null;
			}
			float num2 = Vector3.Distance(gameObject.transform.position, position);
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
		Destroy(gunScript.gameObject);
		_gunObj.transform.localScale = Vector3.one;
		GameObject gameObject = Instantiate<GameObject>(gun2Give.gameObject, _gunObj.transform.position, Quaternion.identity, _gunObj.transform);
		gunScript.transform.eulerAngles = Vector3.zero;
		gunScript = (gameObject.GetComponent(typeof(Gun)) as Gun);
		if (PlayerPrefs.HasKey(GameManager.instance.dataManager.specsKeys[3]))
		{
			extraAmmosPercent = PlayerPrefs.GetFloat(GameManager.instance.dataManager.specsKeys[3]);
		}
		extraAmmos = (int)((float)gunScript.ammos * (extraAmmosPercent / 100f));
		SetGunProperties();
		GameManager.instance.UiManager.SetGunName(gun2Give.gunName);
		GameManager.instance.UiManager.SetAmmoStats(ammos, gunScript.ammos);
	}

	public void ThrowGun(GameObject gun2throw)
	{
		if (ammos > 0 || gunScript == null || gunScript.CompareTag("Knife"))
		{
			return;
		}
		Destroy(gunScript);
		GameObject gameObject = Instantiate<GameObject>(gun2throw, _firePos.position, Quaternion.identity);
		gameObject.GetComponent<Rigidbody2D>().AddForce(_firePos.right * _gunThrowForce, ForceMode2D.Impulse);
		gameObject.GetComponent<Rigidbody2D>().angularVelocity = 1000f;
		ClearAllLines();
		GiveWeapon(knife);
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
		if (gunScript.weaponType == WeaponType.Knife)
		{
			return;
		}
		if (!_isRecharged || ammos <= 0)
		{
			return;
		}
		//gunScript.audioSource.pitch = Random.Range(0.6f, 1.5f);
		gunScript.audioSource.PlayOneShot(gunScript.shotSound);
		GameManager.instance.ShakeOnce();
		rb.AddForce(-_firePos.right * _recoilForce, ForceMode2D.Impulse);
		SpawnBullet();
		_isRecharged = false;
		_currentRecharge = 1f;
		if (GameManager.instance.isGameStarted)
		{
			ammos--;
		}
		if (ammos <= 0)
		{
			GameManager.instance.soundManager._vfxAudioSource.PlayOneShot(gunScript.noAmmoSound);
		}
		GameManager.instance.UiManager.SetAmmoStats(ammos, gunScript.ammos + extraAmmos);
	}

	private void SpawnBullet()
	{
		GameObject gameObject = Instantiate<GameObject>(gunScript.bullet, _firePos.position, _gunObj.transform.rotation);
		if (gunScript.weaponType == WeaponType.Shotgun)
		{
			Bullet[] componentsInChildren = gameObject.GetComponentsInChildren<Bullet>();
			gameObject.transform.localScale *= _bulletScaleModifier;
			Debug.Log(componentsInChildren.Length);
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
		float num = (float)_damage * damageModifier;
		_damage = (int)num;
		curBullet.damage = _damage;
		curBullet.bulletSpeed = _bulletSpeed;
		curBullet.shootingScript = this;
		curBullet.parent = base.gameObject;
		curBullet.lifeTime = _bulletLifetime;
		curBullet.criticalDamageChance = criticalDamageChance;
		curBullet.explosionParticle.startColor = _spriteRenderer.color;
		curBullet.trailRenderer.startWidth *= _bulletScaleModifier;
		SetBulletColor(curBullet);
		Instantiate<ParticleSystem>(shotParticle, _firePos.position, Quaternion.identity);
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003EBE File Offset: 0x000020BE
	private void SetBulletColor(Bullet bullet_obj)
	{
		bullet_obj.spriteRenderer.color = _spriteRenderer.color;
		bullet_obj.trailRenderer.material.color = _spriteRenderer.color;
	}
	
}
