using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Vector3 direction = Vector3.down;
    public float speed = 5f;

    void Update() => transform.position += direction * speed * Time.deltaTime;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player p = other.GetComponent<Player>();
            if (p != null) p.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
