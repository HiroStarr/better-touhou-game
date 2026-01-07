using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform player;

    [Header("Ring Pattern")]
    public int ringBulletCount = 24;
    public float ringSpeed = 2.5f;
    public float ringFireRate = 0.25f;
    public float ringRotationSpeed = 10f;

    [Header("Rain Pattern")]
    public float rainFireRate = 0.05f;
    public float rainSpeed = 3f;
    public float rainSpread = 5f;

    [Header("Aimed Shot")]
    public float aimedFireRate = 1.5f;
    public float aimedSpeed = 6f;

    float ringAngle;

    void Start()
    {
        StartCoroutine(PhaseController());
    }

    // =========================
    // PHASE SYSTEM
    // =========================
    IEnumerator PhaseController()
    {
        while (true)
        {
            // Phase 1: Ring only
            yield return StartCoroutine(RingPhase(5f));

            // Phase 2: Ring + Rain
            yield return StartCoroutine(RingRainPhase(6f));

            // Phase 3: Aimed punishment
            yield return StartCoroutine(AimedPhase(3f));
        }
    }

    // =========================
    // PHASES
    // =========================
    IEnumerator RingPhase(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            FireRing();
            yield return new WaitForSeconds(ringFireRate);
            timer += ringFireRate;
        }
    }

    IEnumerator RingRainPhase(float duration)
    {
        float ringTimer = 0f;
        float rainTimer = 0f;
        float time = 0f;

        while (time < duration)
        {
            if (ringTimer <= 0f)
            {
                FireRing();
                ringTimer = ringFireRate;
            }

            if (rainTimer <= 0f)
            {
                FireRain();
                rainTimer = rainFireRate;
            }

            ringTimer -= Time.deltaTime;
            rainTimer -= Time.deltaTime;
            time += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator AimedPhase(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            FireAimedShot();
            yield return new WaitForSeconds(aimedFireRate);
            timer += aimedFireRate;
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
            SpawnBullet(angle, ringSpeed);
        }
    }

    void FireRain()
    {
        float x = transform.position.x + Random.Range(-rainSpread, rainSpread);
        Vector3 spawnPos = new Vector3(x, transform.position.y, 0f);

        GameObject b = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        b.GetComponent<Bullet>().velocity = Vector2.down * rainSpeed;
    }

    void FireAimedShot()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        b.GetComponent<Bullet>().velocity = dir * aimedSpeed;
    }

    // =========================
    // HELPERS
    // =========================
    void SpawnBullet(float angleDeg, float speed)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        b.GetComponent<Bullet>().velocity = dir * speed;
    }
}
