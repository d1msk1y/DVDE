using System.Collections.Generic;
using UnityEngine;

public class Box : Destructible
{
    [Header("Drop")]
    [SerializeField] private int dropForce;

    [Header("Drop")]
    [SerializeField] private GameObject xpItem;
    [SerializeField] private GameObject healItem;
    [SerializeField] private GameObject shieldItem;
    [SerializeField] private GameObject gunItem;
    [SerializeField] private Detonator _grenade;

    [Header("Random points")]
    [SerializeField] private bool xpChance;
    [SerializeField] private int xpChanceAmount;
    [SerializeField] private bool gunChance;
    [SerializeField] private int gunChanceAmount;
    [SerializeField] private bool shieldChance;
    [SerializeField] private int shieldChanceAmount;
    [SerializeField] private bool healChance;
    [SerializeField] private int healChanceAmount;
    [SerializeField] private bool grenadeChance;
    [SerializeField] private int grenadeChanceAmount;

    [SerializeField] private GameObject[] randomGuns;

    private bool _isBroken;

    private void Start()
    {
        gunItem = randomGuns[Random.Range(0, randomGuns.Length)];
    }

    protected override void Destruct()
    {
        if (_isBroken) {
            return;
        }
        DropItems();
        _isBroken = true;
        base.Destruct();
    }

    private void DropItems()
    {
        List<GameObject> drop = new List<GameObject>();

        GameObject xpItemInst = Instantiate(xpItem, transform.position, Quaternion.identity);
        GameObject healItemInst = Instantiate(healItem, transform.position, Quaternion.identity);
        GameObject shieldItemInst = Instantiate(shieldItem, transform.position, Quaternion.identity);
        GameObject gunInst = Instantiate(gunItem, transform.position, Quaternion.identity);
        // Detonator grenadeInst = Instantiate(_grenade, transform.position, Quaternion.identity);

        xpItemInst.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f, 1f) * dropForce);
        healItemInst.GetComponent<Rigidbody2D>().AddForce(new Vector2(2f, 1f) * dropForce);
        shieldItemInst.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f, -1f) * dropForce);
        gunInst.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f, -1f) * dropForce);

        if (healChance && GameManager.instance.Chance(0, healChanceAmount) != 1)
            Destroy(healItemInst);
        if (xpChance && GameManager.instance.Chance(0, xpChanceAmount) != 1)
            Destroy(xpItemInst);
        if (shieldChance && GameManager.instance.Chance(0, shieldChanceAmount) != 1)
            Destroy(shieldItemInst);
        if (gunChance && GameManager.instance.Chance(0, gunChanceAmount) != 1)
            Destroy(gunInst);
    }

}
