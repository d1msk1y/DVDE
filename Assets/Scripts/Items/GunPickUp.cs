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
            SoundManager.PlayOneShot(GameManager.instance.soundManager.select);
        }
        player.shootingScript.GiveWeapon(gunProperties);
        SoundManager.PlayOneShot(gunProperties.gunPickUpSound);
        PlayerController.instance.shootingScript.ClearAllLines();
        GameManager.instance.UiManager.pickUpButton.gameObject.SetActive(false);
    }

    internal override void CheckPickUpInput() {
        if (itemType == PickupType.Buyable) {
            base.CheckPickUpInput();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && distance < interactRadius) {
            PickUp();
        }
    }

    protected override void Buy()
    {
        base.Buy();
        GameManager.instance.itemsManager.purchasedWeapons =
            GameManager.instance.itemsManager.checkPurchasedGuns();
    }

    protected override void OnReachZoneEnter()
    {
        base.OnReachZoneEnter();

        if (!_speechBaloon) return;
        if (_isSpeechBaloonActive) return;

        _isSpeechBaloonActive = true;
        speechNPC.Say(speech);
    }

    protected override void OnReachZoneExit()
    {
        base.OnReachZoneExit();

        if (!_speechBaloon) return;
        if (!_isSpeechBaloonActive) return;

        _isSpeechBaloonActive = false;

        speechNPC.speechBallon.PopDown();
    }
}
