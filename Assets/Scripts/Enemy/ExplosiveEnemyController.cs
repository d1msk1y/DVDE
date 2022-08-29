using UnityEngine;

[RequireComponent(typeof(Explosive))]
public class ExplosiveEnemyController : EnemyController
{
	private Explosive _explosive;

	internal override void Awake()
	{
		base.Awake();
		_explosive = GetComponent<Explosive>();
	}
	
	internal override void Die()
	{
		base.Die();
		_explosive.Detonate();
	}
}
