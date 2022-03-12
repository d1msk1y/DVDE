using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;
    public int shield;
    public int maxShield;
    public GameObject damageStats;
    public HealthBar healthBar;
    public GameObject shieldBar;

    public Color shieldColorDamage;
    public Color healthColorDamage;
    public Color startColor;

    internal int startShield;

    private void Start()
    {
        startShield = shield;
        health = maxHealth;
        healthBar.gameObject.SetActive(false);
        startColor = GetComponent<SpriteRenderer>().color;
    }

    private void Update()
    {
        if (health != maxHealth || shield != startShield)
        {
            healthBar.gameObject.SetActive(true);
        }

    }

    public void TakeDamage(int damage, SpriteRenderer spriteRenderer)
    {
        if(shield > 0)
        {
            shield -= damage;
            StartCoroutine(colorPop(spriteRenderer, shieldColorDamage));
            if (shield < 0)
                shield = 0;
            return;
        }

        StartCoroutine(colorPop(spriteRenderer, healthColorDamage));

        health -= damage;
    }

    public IEnumerator colorPop(SpriteRenderer spriteRenderer, Color color)
    {
        spriteRenderer.color = color;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = startColor;
    }

    public void Heal(int healthAmount, ItemType itemType)
    {
        if (itemType == ItemType.Shield)
        {
            shield += healthAmount;
            if (shield > maxShield)
                shield = maxShield;
        }

        health += healthAmount;
        if (health > maxHealth)
            health = maxHealth;
    }

    public void ResetHealth()
    {
        health = maxHealth;
        shield = maxShield;
        GetComponent<SpriteRenderer>().color = startColor;
    }

}
