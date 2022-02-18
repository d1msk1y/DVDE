using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFrame : MonoBehaviour
{

    private PickupAble _pickupAble;

    private void Start()
    {
        StartCoroutine(SetColorDelay());
    }

    private IEnumerator SetColorDelay()
    {
        yield return new WaitForEndOfFrame();
        _pickupAble = GetComponentInParent<PickupAble>();

        if(_pickupAble.itemType == PickupType.Buyable && _pickupAble.isBought == 0 && _pickupAble.isUnlocked == 1 ||
            _pickupAble.itemType == PickupType.UpgradeAble && _pickupAble.isBought != 7 && _pickupAble.isUnlocked == 1)
        {
            GetComponent<SpriteRenderer>().color = GameManager.instance.itemsManager.unBoughtColor;
        }

    }

}
