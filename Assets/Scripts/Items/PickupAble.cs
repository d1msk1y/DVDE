using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PickupType
{
    Default,
    Buyable,
    UpgradeAble
}

public class PickupAble : Interactable
{
    //Properties
    public float scaleUpAmount;
    public bool isAutomized;
    public PickupType itemType;

    [Header("GFX")]
    public Sprite outlinedSprite;
    public Sprite defaultSprite;

    [Header("Buyable")]
    public int isUnlocked;//0 - isn't buyable; 1 - buyable
    public int price;
    public string BoughtPrefsKey;
    public int isBought;//0 - isn't bougt; 1 - bought

    //References
    internal Vector3 startScale;
    public SpriteRenderer spriteRenderer;
    internal Animator animator;
    private bool isInReachZone = false;
    private ItemFrame itemFrame;
    [SerializeField] internal Canvas itemCanvas;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(BoughtPrefsKey))
        {
            isBought = PlayerPrefs.GetInt(BoughtPrefsKey);
        }
        if (GetComponent<Animator>() != null)
            animator = GetComponent<Animator>();

    }

    protected new void Start()
    {
        base.Start();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color startColor = spriteRenderer.color;

        if (itemType == PickupType.Buyable && isUnlocked == 1 && isBought == 0 ||
            itemType == PickupType.UpgradeAble && isUnlocked == 1)
        {
            itemCanvas = Instantiate(itemCanvas, transform.position, Quaternion.identity, transform);
            itemCanvas.GetComponentInChildren<Text>().text = price + "$";
        }

        startScale = transform.localScale;

        if (isUnlocked == 0)
        {
            spriteRenderer.color = GameManager.instance.itemsManager.unActiveClothColor;
        }
        else
        {
            if (spriteRenderer != null)
                spriteRenderer.color = startColor;
        }

        if (GetComponentInChildren<ItemFrame>() != null)
        {
            itemFrame = GetComponentInChildren<ItemFrame>();
        }
    }

    protected new void Update()
    {
        if (isUnlocked == 0 && itemType == PickupType.Buyable)
            return;
        base.Update();
        if (Input.GetKeyDown(KeyCode.E) && distance < interactRadius)
        {
            PickUp();
        }
    }


    private IEnumerator ScaleDown()
    {
        while (transform.localScale != startScale && !isInReachZone)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, 0.1f);
            yield return null;
        }
    }
    private IEnumerator ScaleUp()
    {
        while (transform.localScale != Vector3.one * scaleUpAmount && isInReachZone)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startScale * scaleUpAmount, 0.05f);
            yield return null;
        }
    }
    public override void OnReachZoneEnter()
    {
        if (isInReachZone)
            return;
        isInReachZone = true;
        StartCoroutine(ScaleUp());
        spriteRenderer.sprite = outlinedSprite;
        GameManager.instance.UiManager.pickUpButton.gameObject.SetActive(true);
        GameManager.instance.UiManager.toPickup = this;

        if (animator != null)
            animator.SetBool("isNear", true);
    }

    public override void OnReachZoneExit()
    {
        if (!isInReachZone)
            return;
        isInReachZone = false;
        StartCoroutine(ScaleDown());
        spriteRenderer.sprite = defaultSprite;
        GameManager.instance.UiManager.pickUpButton.gameObject.SetActive(false);

        if (animator != null)
            animator.SetBool("isNear", false);
    }

    public override void PickUp()
    {
        if (isBought == 1 && itemType == PickupType.Buyable || itemType == PickupType.Default)
        {
            base.PickUp();
            if (isDestroyable)
            {
                Instantiate(GameManager.instance.itemsManager.pickUpParticle, transform.position, Quaternion.identity);
            }
            else
            {
                ParticleSystem particle = Instantiate(GameManager.instance.itemsManager.pickUpParticle, transform.position, Quaternion.identity);
                particle.transform.localScale = particle.transform.localScale * 1.5f;
            }
        }
        if (isUnlocked == 1 && isBought == 0 && GameManager.instance.scoreManager.TotalCoins >= price)
        {
            Buy();

            if (itemType == PickupType.Buyable)
            {
                Debug.Log(transform.name + " Bought!");
                Destroy(itemCanvas);
                if (itemFrame != null)
                    itemFrame.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public virtual void Buy()
    {
        GameManager.instance.scoreManager.TotalCoins -= price;
        GameManager.instance.statsManager.spentCoins += price;
        GameManager.instance.UiManager.UpdateCostTxts();

        GameManager.instance.UiManager.UpdateCostTxts();
        PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[2], GameManager.instance.statsManager.spentCoins);

        GameManager.instance.statsManager.UpdateStats();

        isBought = 1;
        PlayerPrefs.SetInt(BoughtPrefsKey, isBought);
        PlayerPrefs.SetInt("Total coins", GameManager.instance.scoreManager.TotalCoins);

        Instantiate(GameManager.instance.itemsManager.buyParticle, transform.position, Quaternion.identity);

    }

    public void Unlock()
    {
        isUnlocked = 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
