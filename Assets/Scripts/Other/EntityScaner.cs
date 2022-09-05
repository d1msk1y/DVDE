using System.Collections.Generic;
using UnityEngine;

public class EntityScanner
{
	private readonly float _radius;
	private readonly LayerMask _entityFilter;
	private readonly Transform _transform;

	public EntityScanner (float radius, LayerMask entityFilter, Transform transform)
	{
		_radius = radius;
		_entityFilter = entityFilter;
		_transform = transform;
	}

	public List<EntityHealth> GetEntitiesInRadius()
	{
		if (_transform == null)
			return null;

		var colliders = Physics2D.OverlapCircleAll(_transform.position, _radius, _entityFilter);

		return CheckEntities(colliders);
	}

	public EntityHealth GetClosestEntity (List<EntityHealth> entities)
	{
		if (_transform == null)
			return null;

		EntityHealth result = null;
		var num = float.PositiveInfinity;
		var position = _transform.position;
		foreach (var entity in entities) {
			if (entity == null) {
				return null;
			}
			var num2 = Vector3.Distance(entity.transform.position, position);
			if (!(num2 < num)) continue;
			result = entity;
			num = num2;
		}
		return result;
	}

	private static List<EntityHealth> CheckEntities (IEnumerable<Collider2D> colliders)
	{
		var entities = new List<EntityHealth>();
		foreach (var collider in colliders) {
			if (collider.TryGetComponent(out EntityHealth entityHealth)) {
				entities.Add(entityHealth);
			}
		}
		return entities;
	}
}
