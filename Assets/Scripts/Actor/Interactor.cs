using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Interactor : MonoBehaviour {
	private PickupAble[] _interactables;
	public List<PickupAble> Interactables {
		get {
			_interactables = FindObjectsOfType<PickupAble>();
			return _interactables.OrderBy(distanceToPlayer => distanceToPlayer.distance).ToList();
		}
	}

	private void TryInteract() {
		PickupAble closestPickupAble = GetClosestInteractable();
		if (closestPickupAble.IsUnlocked == 0 && closestPickupAble.itemType == PickupType.Buyable)
			return;
		closestPickupAble.PickUp();
	}
	

	private PickupAble GetClosestInteractable() {
		for (var i = 0; i < Interactables.Count; i++) {
			if (Interactables[i].destroyByLifetime) {
				return Interactables[i];
			}	
		}
		return Interactables[0];
	}
}

