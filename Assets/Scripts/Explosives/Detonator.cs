using Unity.Mathematics;
using UnityEngine;

public abstract class Detonator : MonoBehaviour
{
	[Header("Parameters")]
	[SerializeField] internal float damageRadius;
	[SerializeField] internal LayerMask _vulnerable;

	[Header("SFX")]
	[SerializeField] private ParticleSystem _explosionParticleSystem;

	internal EntityScanner entityScanner;

	internal virtual void OnEnable()
	{
		entityScanner = new EntityScanner(damageRadius, _vulnerable, transform);
	}

	public virtual void Detonate()
	{
		//Detonation algorithm
		if (_explosionParticleSystem != null)
			Instantiate(_explosionParticleSystem, transform.position, quaternion.identity).transform.localScale = Vector3.one * damageRadius;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, damageRadius);
	}
}
