using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 4f;
    public Vector2 direction = Vector2.down;

    void Update()
    {
        transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);
    }

    // Destroy bullet when off-screen
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null && !player.isInvincible)
        {
            player.TakeDamage(1); // Deal damage
            Destroy(gameObject);
        }
    }
}
