using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothSlotController : MonoBehaviour
{
    public ClothSlot hatSlot;
    public ClothSlot glassesSlot;
    public int glassesIndex;
    public int hatIndex;

    public bool isRandomized;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Hat index")) hatIndex = PlayerPrefs.GetInt(GameManager.instance.itemsManager.hatIndexPrefsKey);
        else hatIndex = -1;
        if (PlayerPrefs.HasKey("Glasses index")) glassesIndex = PlayerPrefs.GetInt(GameManager.instance.itemsManager.glassesIndexPrefsKey);
        else glassesIndex = -1;

        hatSlot.clothSlotController = this;
        glassesSlot.clothSlotController = this;
    }
}
