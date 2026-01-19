using UnityEngine;

public class FairyShooter : MonoBehaviour
{
    public enum Pattern
    {
        Spread,
        Circle,
        Spiral,
        Aimed,
        Flower
    }

    [Header("Pattern")]
    public Pattern pattern = Pattern.Spread;
    public GameObject bulletPrefab;

    [Header("Timing")]
    public float fireRate = 1f;
    public float bulletSpeed = 4f;

    [Header("Spread / Circle / Flower")]
    public int bulletCount = 8;
    public float spreadAngle = 60f;

    [Header("Rotation")]
    public float spiralSpeed = 120f; // degrees per second

    float fireTimer;
    float angle;
    Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Fire();
        }

        // continuous rotation for spiral / flower
        if (pattern == Pattern.Spiral || pattern == Pattern.Flower)
        {
            angle += spiralSpeed * Time.deltaTime;
        }
    }

    void Fire()
    {
        switch (pattern)
        {
            case Pattern.Spread: FireSpread(); break;
            case Pattern.Circle: FireCircle(); break;
            case Pattern.Spiral: FireSpiral(); break;
            case Pattern.Aimed: FireAimed(); break;
            case Pattern.Flower: FireFlower(); break;
        }
    }

    // ---------------- PATTERNS ----------------

    void FireSpread()
    {
        if (bulletCount <= 1)
        {
            SpawnBullet(Vector2.down);
            return;
        }

        float start = -spreadAngle * 0.5f;
        float step = spreadAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float a = start + step * i;
            SpawnBullet(AngleToDir(a));
        }
    }

    void FireCircle()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float a = (360f / bulletCount) * i;
            SpawnBullet(AngleToDir(a));
        }
    }

    void FireSpiral()
    {
        SpawnBullet(AngleToDir(angle));
    }

    void FireAimed()
    {
        if (!player) return;
        Vector2 dir = (player.position - transform.position).normalized;
        SpawnBullet(dir);
    }

    void FireFlower()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float a = (360f / bulletCount) * i + angle;
            SpawnBullet(AngleToDir(a));
        }
    }

    // ---------------- HELPERS ----------------

    void SpawnBullet(Vector2 dir)
    {
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        EnemyBullet bullet = b.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.direction = dir;   // Use direction from EnemyBullet.cs
            bullet.speed = bulletSpeed;
        }
    }

    // 0° = DOWN (vertical shmup standard)
    Vector2 AngleToDir(float angle)
    {
        float rad = (angle - 90f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
