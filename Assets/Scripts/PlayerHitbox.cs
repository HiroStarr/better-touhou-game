using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    bool dead = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hitbox collided with: " + other.name + " Tag: " + other.tag);

        if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            Player player = GetComponentInParent<Player>();
            if (player != null)
                player.TakeDamage(1);
        }
    }

}
