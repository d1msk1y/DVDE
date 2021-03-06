using System.Collections;
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

    [Header("Random points")]
    [SerializeField] private bool xpChance;
    [SerializeField] private int xpChanceAmount;
    [SerializeField] private bool gunChance;
    [SerializeField] private int gunChanceAmount;
    [SerializeField] private bool shieldChance;
    [SerializeField] private int shieldChanceAmount;
    [SerializeField] private bool healChance;
    [SerializeField] private int healChanceAmount;

    [SerializeField] private GameObject[] randomGuns;

    private void Start()
    {
        gunItem = randomGuns[Random.Range(0, randomGuns.Length)];
    }

    public override void Destruct()
    {
        DropItems();
        base.Destruct();
    }

    private void DropItems()
    {
        GameObject xpItemInst = Instantiate(xpItem, transform.position, Quaternion.identity);
        GameObject healItemInst = Instantiate(healItem, transform.position, Quaternion.identity);
        GameObject shieldItemInst = Instantiate(shieldItem, transform.position, Quaternion.identity);
        GameObject gunInst = Instantiate(gunItem, transform.position, Quaternion.identity);

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
