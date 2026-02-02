using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossAttack : MonoBehaviour
{
    public Transform player;
    public GameObject ringBulletPrefab, rainBulletPrefab, aimedBulletPrefab;

    [Header("Ring Attack")]
    public int ringBulletCount = 24;
    public float ringSpeed = 2.5f, ringFireRate = 0.25f, ringRotationSpeed = 10f;
    public int ringBurstCount = 3;
    public float ringBurstPause = 1.5f;

    [Header("Rain Attack")]
    public float rainFireRate = 0.05f, rainSpeed = 3f, rainSpread = 5f;

    [Header("Aimed Attack")]
    public float aimedFireRate = 1.2f, aimedSpeed = 6f;

    [Header("After Death")]
    public Dialogue deathDialogue;
    public string nextSceneName = "Stage2";

    private Enemy enemyHealth;
    private Coroutine phaseRoutine;
    private bool dying = false;

    private int ringBurstShotsLeft;
    private float ringBurstCooldown;
    private float ringAngle;

    void Start()
    {
        enemyHealth = GetComponent<Enemy>();

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        ResetRingBurst();
        phaseRoutine = StartCoroutine(PhaseController());
    }

    IEnumerator PhaseController()
    {
        while (!dying && !enemyHealth.IsDead)
        {
            ResetRingBurst();
            yield return StartCoroutine(RingPhase(5f));

            ResetRingBurst();
            yield return StartCoroutine(RingRainPhase(6f));

            yield return StartCoroutine(AimedPhase(4f));
        }

        if (!dying && enemyHealth.IsDead)
            StartCoroutine(DeathSequence());
    }

    void ResetRingBurst()
    {
        ringBurstShotsLeft = ringBurstCount;
        ringBurstCooldown = 0f;
    }

    IEnumerator RingPhase(float duration)
    {
        float t = 0f;
        while (t < duration && !enemyHealth.IsDead)
        {
            UpdateRingBurst(Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator RingRainPhase(float duration)
    {
        float t = 0f;
        float rainTimer = 0f;

        while (t < duration && !enemyHealth.IsDead)
        {
            UpdateRingBurst(Time.deltaTime);

            if (rainTimer <= 0f)
            {
                FireRain();
                rainTimer = rainFireRate;
            }

            rainTimer -= Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator AimedPhase(float duration)
    {
        float t = 0f;
        while (t < duration && !enemyHealth.IsDead)
        {
            FireAimedShot();
            yield return new WaitForSeconds(aimedFireRate);
            t += aimedFireRate;
        }
    }

    void UpdateRingBurst(float dt)
    {
        if (ringBurstCooldown > 0f)
        {
            ringBurstCooldown -= dt;
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
        Vector3 pos = new Vector3(x, transform.position.y, 0f);

        GameObject b = Instantiate(rainBulletPrefab, pos, Quaternion.identity);
        EnemyBullet bullet = b.GetComponent<EnemyBullet>();
        if (bullet)
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
        if (bullet)
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
        if (bullet)
        {
            bullet.direction = dir;
            bullet.speed = speed;
        }
    }

    IEnumerator DeathSequence()
    {
        dying = true;

        if (phaseRoutine != null)
            StopCoroutine(phaseRoutine);

        yield return new WaitForSeconds(0.5f);

        if (deathDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(deathDialogue);
            while (DialogueManager.Instance.IsDialoguePlaying)
                yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
