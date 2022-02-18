using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [Header("Destructible properties")]
    public int health;

    [Header("GFX")]
    public ParticleSystem explosion;

    internal int currentHealth;

    private void Awake()
    {
        currentHealth = health;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
            Destruct();
    }

    public virtual void Destruct()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
