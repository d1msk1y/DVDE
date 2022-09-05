using UnityEngine;
public class GrenadePickUp : PickupAble
{
	[SerializeField] private Detonator _grenade;

	public override void PickUp()
	{
		if (itemType == PickupType.Buyable && GameManager.instance.scoreManager.TotalCoins < price && isBought == 0)
			return;
		
		base.PickUp();
		
		player.GetComponent<GrenadeSlot>().SetGrenade(_grenade);
		
	}
}