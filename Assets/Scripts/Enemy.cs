using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP = 3;
    public bool isBoss = false;
    public GameObject deathEffect;

    int currentHP;

    void Awake() => currentHP = maxHP;

    public void TakeDamage(int dmg)
    {
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerBullet")) return;

        TakeDamage(1);
        Destroy(other.gameObject);
    }
}
