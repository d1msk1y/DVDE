using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothPickup : PickupAble
{
    [Header("Cloth properties")]
    public int clothIndex;
    public ClothType clothType;
    public GameObject clothItem;

    private ClothSlotController clothSlotController;
    private ClothSlot clotheTypeSlot;
    private string prefsIndex;

    public new void Start()
    {
        base.Start();

        clothSlotController = player.GetComponent<ClothSlotController>();

        if (clothType == ClothType.Hat)
        {
            clotheTypeSlot = clothSlotController.hatSlot;
            prefsIndex = GameManager.instance.itemsManager.hatIndexPrefsKey;
        }
        if (clothType == ClothType.Glasses)
        {
            clotheTypeSlot = clothSlotController.glassesSlot;
            prefsIndex = GameManager.instance.itemsManager.glassesIndexPrefsKey;
        }
    }

    public override void PickUp()
    {
        base.PickUp();
        if(isBought == 1 && itemType == PickupType.Buyable)
        {
            clotheTypeSlot.GiveCloth(clothItem, clothIndex, prefsIndex, clothType);
        }
    }
}
