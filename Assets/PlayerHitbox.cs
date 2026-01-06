using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    bool dead = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (dead) return;

        if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;
        Debug.Log("PLAYER DEAD");

        // Optional: death effect here

        Destroy(transform.root.gameObject);
    }
}
