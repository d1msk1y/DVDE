using FMOD;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Health, Shield, Experience }

public class HealItem : Interactable
{
    public int useFulAmount;
    public ItemType itemType;
    public float moveSpeed;
    public float pickUpRadius;

    [Header("SFX")]
    [SerializeField] private EventReference _pickupSound;

    private EntityHealth _playerHealth;

    private new void Start()
    {
        base.Start();
        _playerHealth = player.entityHealth;

        if (itemType == ItemType.Shield)
        {
            useFulAmount = GameManager.instance.itemsManager.shieldUseful;
        }
        else if (itemType == ItemType.Health)
            useFulAmount = GameManager.instance.itemsManager.healUseful;
    }
    private new void Update()
    {
        base.Update();
        if (player == null)
            return;

        if (distance < pickUpRadius)
        {
            PickUp();
        }
    }

    public override void PickUp()
    {
        SoundManager.PlayOneShot(_pickupSound);

        switch (itemType)
        {
            case ItemType.Experience:

                base.PickUp();
                PickUpXP();

                break;
            case ItemType.Health:

                base.PickUp();
                _playerHealth.Heal(useFulAmount, itemType);

                break;
            case ItemType.Shield:

                base.PickUp();
                _playerHealth.Heal(useFulAmount, itemType);

                break;
        }
    }

    private void PickUpXP()
    {
        GameManager.instance.scoreManager.AddScore(useFulAmount);
    }

    protected override void OnReachZoneEnter()
    {
        transform.position = Vector2.Lerp(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }
}