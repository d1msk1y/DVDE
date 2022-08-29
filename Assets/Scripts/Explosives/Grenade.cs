using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Explosive))]
public class Grenade : MonoBehaviour
{
	[Header("Parameters")]
	[SerializeField] private float _detonationTime;
	
	private Explosive _explosive;

	private void Start()
	{
		_explosive = GetComponent<Explosive>();
	}

	private void OnEnable()
	{
		StartCoroutine(Activate());
	}

	private IEnumerator Activate()
	{
		//Do some activation stuff(sfx, vfx)
		yield return new WaitForSeconds(_detonationTime);
		_explosive.Detonate();
	}
}
