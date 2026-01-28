using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP = 3;
    public bool isBoss = false;
    public bool invincible = false;
    public GameObject deathEffect;

    int currentHP;
    public int CurrentHP => currentHP;

    void Awake() => currentHP = maxHP;

    public void TakeDamage(int dmg)
    {
        if (invincible) return;

        currentHP -= dmg;
        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
