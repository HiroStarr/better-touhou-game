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
    private int currentHealth;
    private bool isDead;

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
        currentHealth = maxHealth;
        StartCoroutine(PhaseController());
    }

    IEnumerator PhaseController()
    {
        while (!isDead)
        {
            yield return StartCoroutine(RingPhase(5f));
            yield return StartCoroutine(RingRainPhase(6f));
            yield return StartCoroutine(AimedPhase(3f));
        }
    }

    IEnumerator RingPhase(float duration)
    {
        float timer = 0f;
        while (timer < duration && !isDead)
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

        while (time < duration && !isDead)
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
        while (timer < duration && !isDead)
        {
            FireAimedShot();
            yield return new WaitForSeconds(aimedFireRate);
            timer += aimedFireRate;
        }
    }

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
        b.GetComponent<Bullet>().velocity = Vector2.down * rainSpeed;
    }

    void FireAimedShot()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        GameObject b = Instantiate(aimedBulletPrefab, transform.position, Quaternion.identity);
        b.GetComponent<Bullet>().velocity = dir * aimedSpeed;
    }

    void SpawnBullet(GameObject prefab, float angleDeg, float speed)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        GameObject b = Instantiate(prefab, transform.position, Quaternion.identity);
        b.GetComponent<Bullet>().velocity = dir * speed;
    }

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
