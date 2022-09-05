using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    public int Health {
        get => _health;
        set
        {
            _health = value;
            if(_health <= 0) OnDie?.Invoke();
        }
    }
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

    public SpriteRenderer spriteRenderer;

    public delegate void EntityHandler();
    public event EntityHandler onDamageEvent;
    public event EntityHandler onDieEvent;
    public UnityEvent OnDie;
    public event EntityHandler onHealEvent;

    internal int startShield;

    private void Start()
    {
        startShield = maxShield;
        Health = maxHealth;
        healthBar.gameObject.SetActive(false);
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
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
        if (shield > 0) {
            var initialShield = shield;
            shield -= damage;
            onDamageEvent?.Invoke();
            StartCoroutine(colorPop(spriteRenderer, shieldColorDamage));
            
            if (shield < 0) shield = 0;
            if(initialShield >= damage) return;
            damage -= initialShield;
        }

        StartCoroutine(colorPop(spriteRenderer, healthColorDamage));

        onDamageEvent?.Invoke();
        Health -= damage;
    }

    private IEnumerator colorPop(SpriteRenderer spriteRenderer, Color color)
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

        onHealEvent?.Invoke();
    }

    public void ResetHealth()
    {
        Health = maxHealth;
        shield = maxShield;
        GetComponent<SpriteRenderer>().color = startColor;
    }
}