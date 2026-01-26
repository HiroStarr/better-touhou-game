using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    public Vector3 direction = Vector3.up;
    public float speed = 10f;

    void Update()
    {
        if (GameState.Instance.GameplayLocked) return;

        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}

