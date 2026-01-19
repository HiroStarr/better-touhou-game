using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    public float speed = 10f;
    public Vector3 direction = Vector3.up;
    public int damage = 1;

    void Update()
    {
        // Move the bullet
        transform.position += direction.normalized * speed * Time.deltaTime;

        // Destroy if off-screen
        if (Mathf.Abs(transform.position.y) > 20f || Mathf.Abs(transform.position.x) > 20f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Damage bosses
        BossAttack boss = other.GetComponent<BossAttack>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
