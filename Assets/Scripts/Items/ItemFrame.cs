using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour
{

    private PickupAble _pickupAble;

    private void Start()
    {
        SetColor();
        GameManager.instance.scoreManager.onCoinsChange += SetColor;
    }

    private void SetColor() => StartCoroutine(SetColorCoroutine());
    
    public IEnumerator SetColorCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _pickupAble = GetComponentInParent<PickupAble>();

        if(_pickupAble.itemType == PickupType.Buyable && _pickupAble.isBought == 0 && _pickupAble.IsUnlocked == 1 ||
            _pickupAble.itemType == PickupType.UpgradeAble && _pickupAble.isBought != 7 && _pickupAble.IsUnlocked == 1)
        {
            GetComponent<SpriteRenderer>().color = GameManager.instance.itemsManager.unBoughtColor;
            if (_pickupAble.price > GameManager.instance.scoreManager.TotalCoins)
                _pickupAble.itemCanvas.GetComponentInChildren<Text>().color = GameManager.instance.itemsManager.notEnoughCoinsColor;
            else _pickupAble.itemCanvas.GetComponentInChildren<Text>().color = GameManager.instance.itemsManager.unBoughtColor;
        }
        if(_pickupAble.IsUnlocked == 0) GetComponent<SpriteRenderer>().color = GameManager.instance.itemsManager.unActiveClothColor;

    }

}
