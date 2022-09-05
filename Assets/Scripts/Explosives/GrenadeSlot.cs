using UnityEngine;

public class GrenadeSlot : MonoBehaviour
{
	[SerializeField] private Detonator _selectedGrenade;
	[SerializeField] private int _grenadesLeft = 1;

	public void AddGrenade() => _grenadesLeft++;
	
	public void SetGrenade (Detonator grenade) => _selectedGrenade = grenade;
	
	public Detonator GetGrenade()
	{
		if (_grenadesLeft <= 0) return null;
		_grenadesLeft--;
		return _selectedGrenade;
	}
}
