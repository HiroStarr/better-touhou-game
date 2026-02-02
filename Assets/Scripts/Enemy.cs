using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP = 3;
    public bool isBoss = false;
    public bool invincible = false;
    public GameObject deathEffect;

    int currentHP;
    public int CurrentHP => currentHP;

    public bool IsDead { get; private set; } = false; // ✅ added

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        if (invincible || IsDead) return;

        currentHP -= dmg;

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;

        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (!isBoss)
        {
            // ✅ Normal enemies die instantly
            Destroy(gameObject);
        }
        else
        {
            // ✅ Boss stays alive for dialogue / death sequence
            // Disable collision so it can’t be hit anymore
            Collider2D col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }
    }
}
