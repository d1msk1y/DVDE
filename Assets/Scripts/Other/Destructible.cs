using UnityEngine;

public class Destructible : MonoBehaviour {
    [Header("Destructible properties")]
    public int health;

    [Header("GFX")]
    public ParticleSystem explosion;

    private int currentHealth;

    private void Awake() {
        currentHealth = health;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if(currentHealth <= 0)
            Destruct();
    }

    protected virtual void Destruct() {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}