using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : PickupAble
{
    [Header("Tip")]
    [Multiline]
    public string speech;
    public SpeechNPC speechNPC;

    private bool _isSpeechBaloonActive;
    [SerializeField] private bool _speechBaloon;

    [Space(10)]
    public Gun gunProperties;

    private new void Update()
    {
        base.Update();
        if (player.GetComponent<PlayerController>().shootingScript.ammos <= 0 && distance < interactRadius)
        {
            PickUp();
        }
    }

    public override void PickUp()
    {
        if (itemType == PickupType.Buyable && GameManager.instance.scoreManager.TotalCoins < price && isBought == 0)
            return;

        base.PickUp();
        if (itemType == PickupType.Buyable)
        {
            GameManager.instance.itemsManager.pickedGun = gunProperties.gunIndex;
            PlayerPrefs.SetInt("Picked gun", gunProperties.gunIndex);
        }
        player.GetComponent<ActorShooting>().GiveWeapon(gunProperties);
        GameManager.instance.soundManager._vfxAudioSource.PlayOneShot(gunProperties.gunPickUpSound, 2f);
        PlayerController.instance.shootingScript.ClearAllLines();
        GameManager.instance.UiManager.pickUpButton.gameObject.SetActive(false);
    }

    public override void Buy()
    {
        base.Buy();
        GameManager.instance.itemsManager.purchasedWeapons =
            GameManager.instance.itemsManager.checkPurchasedGuns();
    }

    public override void OnReachZoneEnter()
    {
        base.OnReachZoneEnter();

        if (_isSpeechBaloonActive && !_speechBaloon)
            return;

        _isSpeechBaloonActive = true;
        speechNPC.Say(speech);
    }

    public override void OnReachZoneExit()
    {
        base.OnReachZoneExit();

        if (!_isSpeechBaloonActive || !_speechBaloon)
            return;

        _isSpeechBaloonActive = false;

        speechNPC.speechBallon.PopDown();
    }
}
