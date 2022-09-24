using UnityEngine;
using UnityEngine.UI;

public class LockSerializer : MonoBehaviour
{
	[SerializeField]private GameObject _lockCanvas;
	private GameObject _lockCanvasInstance;
	
	private PickupAble _pickupAble;

	private void Start()
	{
		_pickupAble = GetComponent<PickupAble>();
		if(!ValidateLock()) return;
		GameManager.instance.scoreManager.onLevelUp += UpdateLock;
		SetLock();
	}
	
	private bool ValidateLock()
	{
		if (_pickupAble.IsUnlocked == 0 && _pickupAble.LvlToUnlock > 0) return true;
		Destroy(_lockCanvasInstance);
		return false;
	}

	private void SetLock()
	{
		_lockCanvasInstance = Instantiate(_lockCanvas, transform.position, Quaternion.identity, transform);
		UpdateLock();
	}

	private void UpdateLock()
	{
		if(ValidateLock())
			_lockCanvasInstance.GetComponentInChildren<Text>().text = "Level " + _pickupAble.LvlToUnlock;
	}
}