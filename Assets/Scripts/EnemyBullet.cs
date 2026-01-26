using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{
    public Vector2 direction = Vector2.down;
    public float speed = 5f;
    public int damage = 1;

    public float maxLifetime = 8f;
    public float offscreenPadding = 0.1f;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        Destroy(gameObject, maxLifetime);
    }

    void Update()
    {
        if (GameState.Instance.GameplayLocked) return;

        transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);

        if (IsOffScreen())
            Destroy(gameObject);
    }

    bool IsOffScreen()
    {
        Vector3 vp = cam.WorldToViewportPoint(transform.position);
        return vp.x < -offscreenPadding || vp.x > 1 + offscreenPadding ||
               vp.y < -offscreenPadding || vp.y > 1 + offscreenPadding;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player p = other.GetComponent<Player>();
        if (p != null)
            p.TakeDamage(damage);

        Destroy(gameObject);
    }
}
