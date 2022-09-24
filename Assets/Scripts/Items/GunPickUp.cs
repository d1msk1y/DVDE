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
        if (player.shootingScript.ammos <= 0 && distance < interactRadius)
        {
            PickUp();
        }
    }

    public override void PickUp()
    {
        if (itemType == PickupType.Buyable && GameManager.instance.scoreManager.TotalCoins < price && isBought == 0 || itemType == PickupType.Buyable && IsUnlocked == 0)
            return;

        base.PickUp();
        if (itemType == PickupType.Buyable)
        {
            GameManager.instance.itemsManager.pickedGun = gunProperties.gunIndex;
            PlayerPrefs.SetInt("Picked gun", gunProperties.gunIndex);
            GameManager.instance.soundManager.PlayVfx(GameManager.instance.soundManager.select);
        }
        player.shootingScript.GiveWeapon(gunProperties);
        GameManager.instance.soundManager._vfxAudioSource.PlayOneShot(gunProperties.gunPickUpSound, 2f);
        PlayerController.instance.shootingScript.ClearAllLines();
        GameManager.instance.UiManager.pickUpButton.gameObject.SetActive(false);
    }

    protected override void Buy()
    {
        base.Buy();
        GameManager.instance.itemsManager.purchasedWeapons =
            GameManager.instance.itemsManager.checkPurchasedGuns();
    }

    public override void OnReachZoneEnter()
    {
        base.OnReachZoneEnter();

        if (!_speechBaloon) return;
        if (_isSpeechBaloonActive) return;

        _isSpeechBaloonActive = true;
        speechNPC.Say(speech);
    }

    public override void OnReachZoneExit()
    {
        base.OnReachZoneExit();

        if (!_speechBaloon) return;
        if (!_isSpeechBaloonActive) return;

        _isSpeechBaloonActive = false;

        speechNPC.speechBallon.PopDown();
    }
}
