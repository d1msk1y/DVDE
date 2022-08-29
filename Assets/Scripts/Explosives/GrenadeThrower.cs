using System;
using Unity.Mathematics;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
	[SerializeField] private float _throwForce;
	
	private GrenadeSlot _grenadeSlot;
	private EntityScanner _entityScanner;

	private void Start()
	{
		_grenadeSlot = GetComponent<GrenadeSlot>();
	}

	public void ThrowGrenade()
	{
		var grenade = _grenadeSlot.GetGrenade();
		if(grenade == null) return;
		
		grenade = Instantiate(grenade, PlayerController.instance.shootingScript._firePos.position, quaternion.identity);
		grenade.GetComponent<Rigidbody2D>().AddForce(PlayerController.instance.shootingScript._firePos.right * _throwForce, ForceMode2D.Impulse);
	}
}
