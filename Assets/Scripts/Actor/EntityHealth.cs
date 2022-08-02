using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int Health { get => _health; set => _health = value; }
    private int _health;

    public int maxHealth = 100;
    public int shield;
    public int maxShield;
    public GameObject damageStats;
    public HealthBar healthBar;
    public GameObject shieldBar;

    public Color shieldColorDamage;
    public Color healthColorDamage;
    public Color startColor;

    public delegate void EntityHandler();
    public event EntityHandler onDamageEvent;
    public event EntityHandler onDieEvent;

    internal int startShield;

    private void Start()
    {
        startShield = shield;
        Health = maxHealth;
        healthBar.gameObject.SetActive(false);
        startColor = GetComponent<SpriteRenderer>().color;
    }

    private void Update()
    {
        if (Health != maxHealth || shield != startShield)
        {
            healthBar.gameObject.SetActive(true);
        }

    }

    public void TakeDamage(int damage, SpriteRenderer spriteRenderer)
    {
        if (shield > 0)
        {
            shield -= damage;
            onDamageEvent?.Invoke();
            StartCoroutine(colorPop(spriteRenderer, shieldColorDamage));
            if (shield < 0)
                shield = 0;
            return;
        }

        StartCoroutine(colorPop(spriteRenderer, healthColorDamage));

        onDamageEvent?.Invoke();
        Health -= damage;
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

        Health += healthAmount;
        if (Health > maxHealth)
            Health = maxHealth;
    }

    public void ResetHealth()
    {
        Health = maxHealth;
        shield = maxShield;
        GetComponent<SpriteRenderer>().color = startColor;
    }
}