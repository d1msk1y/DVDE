using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClothType {Hat, Glasses}
public class ClothSlot : MonoBehaviour
{
    public ClothSlotController clothSlotController;

    public ClothType clothType;

    public Transform slot;
    public GameObject currentCloth;
    public int clotheIndex;//Current cloth index

    private void Awake()
    {
        if(clothType == ClothType.Hat && PlayerPrefs.HasKey("Hat index"))
        {
            clotheIndex = PlayerPrefs.GetInt("Hat index");
        }
        if(clothType == ClothType.Glasses && PlayerPrefs.HasKey("Glasses index"))
        {
            clotheIndex = PlayerPrefs.GetInt("Glasses index");
        }

        clothSlotController = GetComponentInParent<ClothSlotController>();
    }

    private void Start()
    {
        if (clothSlotController.isRandomized)
            GiveRandomClothes();
    }

    public void GiveCloth(GameObject cloth, int clothIndex, string prefsKey, ClothType clothType)
    {
        ClearSlot();
        clotheIndex = clothIndex;//Set an index of given cloth
        if(clothType == ClothType.Hat)
        {
            clothSlotController.hatIndex = clothIndex;
        }
        if(clothType == ClothType.Glasses)
        {
            clothSlotController.glassesIndex = clothIndex;
        }
        if(prefsKey != null)
            PlayerPrefs.SetInt(prefsKey, clothIndex);

        if (clotheIndex == -1)
        {
            ClearSlot();
            return;
        }

        GameObject instantiatedHat = Instantiate(cloth, slot.position, Quaternion.identity, slot);
        currentCloth = instantiatedHat;
         

    }
    public void GiveRandomClothes()
    {
        int randomizeHat = Random.Range(0, GameManager.instance.itemsManager.hats.Length);
        int randomizeGlasses = Random.Range(0, GameManager.instance.itemsManager.glasses.Length);
        if(clothType == ClothType.Glasses)
            GiveCloth(GameManager.instance.itemsManager.glasses[randomizeGlasses], randomizeGlasses, null, ClothType.Glasses);
        if(clothType == ClothType.Hat)
            GiveCloth(GameManager.instance.itemsManager.hats[randomizeHat], randomizeHat, null, ClothType.Hat);
    }
    private void ClearSlot()
    {
        Destroy(currentCloth);
    }
}
