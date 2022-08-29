using UnityEngine;

public class Explosive : Detonator
{
	[SerializeField] internal int _damage;

	public override void Detonate()
	{
		base.Detonate();
		var entities = entityScanner.GetEntitiesInRadius();
		GameManager.instance.ShakeScreen(5);
		Destroy(gameObject);
		foreach (var entity in entities) entity.TakeDamage(_damage, entity.spriteRenderer);
	}
}
