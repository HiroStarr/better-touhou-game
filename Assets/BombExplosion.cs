using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    public float duration = 0.8f;      // Total bomb animation time
    public float maxScale = 3f;        // Visual size growth
    public float maxRadius = 5f;       // Maximum bomb effect radius
    public int damage = 5;             // Damage to enemies

    private SpriteRenderer sr;
    private float timer = 0f;
    private Vector3 origin;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.zero;
        origin = transform.position;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;

        // 1. Expand visual sprite
        transform.localScale = Vector3.one * maxScale * progress;

        // 2. Fade out sprite
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, progress);
            sr.color = c;
        }

        // 3. Calculate expanding effective radius
        float currentRadius = Mathf.Lerp(0f, maxRadius, progress);

        // Destroy enemy bullets inside radius
        EnemyBullet[] bullets = FindObjectsOfType<EnemyBullet>();
        foreach (EnemyBullet b in bullets)
        {
            if (Vector3.Distance(origin, b.transform.position) <= currentRadius)
                Destroy(b.gameObject);
        }

        // Damage enemies inside radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, currentRadius);
        foreach (Collider2D c in hits)
        {
            Enemy enemy = c.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }

        // Destroy explosion after animation
        if (timer >= duration)
            Destroy(gameObject);
    }

    // Optional: draw radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}
