using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : PickupAble
{
    [Space(10)]
    public Gun gunProperties;

    private new void Update()
    {
        base.Update();
        if(player.GetComponent<PlayerController>().shootingScript.ammos <= 0 && distance < interactRadius)
        {
            PickUp();
        }
    }

    public override void PickUp()
    {
        if(itemType == PickupType.Buyable && GameManager.instance.scoreManager.totalCoins < price)
            return;

        base.PickUp();
        if (itemType == PickupType.Buyable)
        {
            GameManager.instance.itemsManager.pickedGun = gunProperties.gunIndex;
            PlayerPrefs.SetInt("Picked gun", gunProperties.gunIndex);
        }
        player.GetComponent<ActorShooting>().GiveWeapon(gunProperties);
        GameManager.instance.soundManager._vfxAudioSource.PlayOneShot(gunProperties.gunPickUpSound,2f);
        PlayerController.instance.shootingScript.ClearAllLines();
        GameManager.instance.UiManager.pickUpButton.gameObject.SetActive(false);        
    }

    public override void Buy()
    {
        base.Buy();
        GameManager.instance.itemsManager.purchasedWeapons =
            GameManager.instance.itemsManager.checkPurchasedGuns();
    }
}
