using UnityEngine;

// KillZone: safer detection for out-of-bounds entities.
// Place this on a GameObject with a BoxCollider2D (Is Trigger = true).
// This uses Physics2D.OverlapBoxAll so it works even if bullets/enemies don't have Rigidbody2D components.
[RequireComponent(typeof(BoxCollider2D))]
public class KillZone : MonoBehaviour
{
    [Tooltip("If true, destroy any collider found in the zone (except Player/PlayerHitbox). If false, only destroy known enemy/bullet types.")]
    public bool destroyAny = false;

    BoxCollider2D box;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        if (box == null)
            Debug.LogError("KillZone requires a BoxCollider2D.");

        if (box != null && !box.isTrigger)
            Debug.LogWarning("KillZone box collider is not marked as Trigger. It is recommended to set Is Trigger = true.");
    }

    void FixedUpdate()
    {
        if (box == null) return;

        Bounds b = box.bounds;
        Collider2D[] hits = Physics2D.OverlapBoxAll(b.center, b.size, 0f);

        foreach (var c in hits)
        {
            if (c == null) continue;
            if (c.gameObject == gameObject) continue; // ignore self
            if (c == box) continue;

            // Ignore player and their hitbox
            if (c.CompareTag("Player") || c.CompareTag("PlayerHitbox"))
                continue;

            GameObject root = c.transform.root.gameObject;
            if (root == gameObject) continue;

            if (destroyAny)
            {
                Destroy(root);
                continue;
            }

            // Only destroy objects that are likely enemies or enemy bullets.
            bool isEnemyType = c.GetComponent<Enemy>() != null || c.GetComponent<Fairy>() != null || c.GetComponent<EnemyBullet>() != null;
            bool hasEnemyTag = c.CompareTag("Enemy") || c.CompareTag("EnemyBullet") || c.CompareTag("Bullet");

            if (isEnemyType || hasEnemyTag)
                Destroy(root);
        }
    }
}
