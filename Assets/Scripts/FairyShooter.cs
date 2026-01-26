using UnityEngine;

public class FairyShooter : MonoBehaviour
{
    public enum Pattern { Spread, Circle, Spiral, Aimed, Flower }
    public Pattern pattern = Pattern.Spread;

    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float bulletSpeed = 4f;

    public int bulletCount = 8;
    public float spreadAngle = 60f;
    public float spiralSpeed = 120f;

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
        if (GameState.Instance.GameplayLocked) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Fire();
        }

        if (pattern == Pattern.Spiral || pattern == Pattern.Flower)
            angle += spiralSpeed * Time.deltaTime;
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

    void FireSpread()
    {
        float start = -spreadAngle * 0.5f;
        float step = spreadAngle / Mathf.Max(1, bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
            SpawnBullet(AngleToDir(start + step * i));
    }

    void FireCircle()
    {
        for (int i = 0; i < bulletCount; i++)
            SpawnBullet(AngleToDir(360f / bulletCount * i));
    }

    void FireSpiral() => SpawnBullet(AngleToDir(angle));

    void FireAimed()
    {
        if (!player) return;
        SpawnBullet((player.position - transform.position).normalized);
    }

    void FireFlower()
    {
        for (int i = 0; i < bulletCount; i++)
            SpawnBullet(AngleToDir(360f / bulletCount * i + angle));
    }

    void SpawnBullet(Vector2 dir)
    {
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        EnemyBullet bullet = b.GetComponent<EnemyBullet>();
        if (bullet)
        {
            bullet.direction = dir;
            bullet.speed = bulletSpeed;
        }
    }

    Vector2 AngleToDir(float angle)
    {
        float rad = (angle - 90f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
