using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    [Header("Clothes")]
    public GameObject[] glasses;
    public GameObject[] hats;

    [Header("Weapons")]
    public Gun[] weapons;
    public List<Gun> purchasedWeapons;
    public GunPickUp[] buyableGuns;
    public int pickedGun;

    [Header("Heals")]
    public int shieldUseful;
    public int healUseful;

    [Header("Items colors")]
    public Color unActiveClothColor;//*Item
    public Color unBoughtColor;//*Item
    public Color notEnoughCoinsColor;//*Item

    [Header("Prefs")]
    public string hatIndexPrefsKey;
    public string glassesIndexPrefsKey;

    [Header("Items")]
    public ParticleSystem buyParticle;
    public ParticleSystem pickUpParticle;
    public Canvas itemCanvas;

    private void Start()
    {
        purchasedWeapons = checkPurchasedGuns();
    }

    public List<Gun> checkPurchasedGuns()
    {
        List<Gun> purchasedGunsCalc = new List<Gun>();
        foreach (GunPickUp gun in buyableGuns)
        {
            if (gun.isBought == 1)
            {
                Gun gunProp = gun.gunProperties;
                purchasedGunsCalc.Add(gunProp);
            }
        }
        return purchasedGunsCalc;
    }
}
