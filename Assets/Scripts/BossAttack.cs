using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Bullet Prefabs")]
    public GameObject ringBulletPrefab;
    public GameObject rainBulletPrefab;
    public GameObject aimedBulletPrefab;

    [Header("Boss Health")]
    public int maxHealth = 300;
    int currentHealth;
    bool isDead;

    [Header("Ring Pattern")]
    public int ringBulletCount = 24;
    public float ringSpeed = 2.5f;
    public float ringFireRate = 0.25f;
    public float ringRotationSpeed = 10f;

    [Header("Ring Burst")]
    public int ringBurstCount = 3;
    public float ringBurstPause = 1.5f;

    [Header("Rain Pattern")]
    public float rainFireRate = 0.05f;
    public float rainSpeed = 3f;
    public float rainSpread = 5f;

    [Header("Aimed Shot")]
    public float aimedFireRate = 1.5f;
    public float aimedSpeed = 6f;

    // ===== Internal state for burst logic =====
    int ringBurstShotsLeft;
    float ringBurstCooldown;
    float ringAngle;

    void Start()
    {
        currentHealth = maxHealth;
        ringBurstShotsLeft = ringBurstCount;
        ringBurstCooldown = 0f;

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        StartCoroutine(PhaseController());
    }

    // =========================
    // PHASE SYSTEM
    // =========================
    IEnumerator PhaseController()
    {
        while (!isDead)
        {
            ResetRingBurst();
            yield return StartCoroutine(RingPhase(5f));

            ResetRingBurst();
            yield return StartCoroutine(RingRainPhase(6f));

            yield return StartCoroutine(AimedPhase(3f));
        }
    }

    void ResetRingBurst()
    {
        ringBurstShotsLeft = ringBurstCount;
        ringBurstCooldown = 0f;
    }

    // =========================
    // PHASES
    // =========================
    IEnumerator RingPhase(float duration)
    {
        float timer = 0f;

        while (timer < duration && !isDead)
        {
            UpdateRingBurst(Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator RingRainPhase(float duration)
    {
        float rainTimer = 0f;
        float time = 0f;

        while (time < duration && !isDead)
        {
            // Ring burst
            UpdateRingBurst(Time.deltaTime);

            // Rain bullets
            if (rainTimer <= 0f)
            {
                FireRain();
                rainTimer = rainFireRate;
            }

            rainTimer -= Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator AimedPhase(float duration)
    {
        float timer = 0f;

        while (timer < duration && !isDead)
        {
            FireAimedShot();
            yield return new WaitForSeconds(aimedFireRate);
            timer += aimedFireRate;
        }
    }

    // =========================
    // BURST HELPER
    // =========================
    void UpdateRingBurst(float deltaTime)
    {
        if (ringBurstCooldown > 0f)
        {
            ringBurstCooldown -= deltaTime;
            return;
        }

        FireRing();
        ringBurstShotsLeft--;

        if (ringBurstShotsLeft <= 0)
        {
            ringBurstShotsLeft = ringBurstCount;
            ringBurstCooldown = ringBurstPause;
        }
        else
        {
            ringBurstCooldown = ringFireRate;
        }
    }

    // =========================
    // PATTERNS
    // =========================
    void FireRing()
    {
        ringAngle += ringRotationSpeed;

        for (int i = 0; i < ringBulletCount; i++)
        {
            float angle = ringAngle + (360f / ringBulletCount) * i;
            SpawnBullet(ringBulletPrefab, angle, ringSpeed);
        }
    }

    void FireRain()
    {
        float x = transform.position.x + Random.Range(-rainSpread, rainSpread);
        Vector3 spawnPos = new Vector3(x, transform.position.y, 0f);

        GameObject b = Instantiate(rainBulletPrefab, spawnPos, Quaternion.identity);
        EnemyBullet bullet = b.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.direction = Vector2.down;
            bullet.speed = rainSpeed;
        }
    }

    void FireAimedShot()
    {
        if (!player) return;

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject b = Instantiate(aimedBulletPrefab, transform.position, Quaternion.identity);
        EnemyBullet bullet = b.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.direction = dir;
            bullet.speed = aimedSpeed;
        }
    }

    void SpawnBullet(GameObject prefab, float angleDeg, float speed)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        GameObject b = Instantiate(prefab, transform.position, Quaternion.identity);
        EnemyBullet bullet = b.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.direction = dir;
            bullet.speed = speed;
        }
    }

    // =========================
    // DAMAGE / DEATH
    // =========================
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
