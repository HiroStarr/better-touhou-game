using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public Transform player;
    public GameObject ringBulletPrefab, rainBulletPrefab, aimedBulletPrefab;
    public int ringBulletCount = 24;
    public float ringSpeed = 2.5f, ringFireRate = 0.25f, ringRotationSpeed = 10f;
    public int ringBurstCount = 3; public float ringBurstPause = 1.5f;
    public float rainFireRate = 0.05f, rainSpeed = 3f, rainSpread = 5f;
    public float aimedFireRate = 1.5f, aimedSpeed = 6f;

    private Enemy enemyHealth;
    private int ringBurstShotsLeft;
    private float ringBurstCooldown;
    private float ringAngle;

    void Start()
    {
        enemyHealth = GetComponent<Enemy>();
        if (enemyHealth == null) { Debug.LogError("Boss needs Enemy component!"); return; }

        ringBurstShotsLeft = ringBurstCount; ringBurstCooldown = 0f;
        if (!player) { GameObject p = GameObject.FindGameObjectWithTag("Player"); if (p) player = p.transform; }
        StartCoroutine(PhaseController());
    }

    IEnumerator PhaseController()
    {
        while (enemyHealth != null && enemyHealth.maxHP > 0)
        {
            ResetRingBurst(); yield return StartCoroutine(RingPhase(5f));
            ResetRingBurst(); yield return StartCoroutine(RingRainPhase(6f));
            yield return StartCoroutine(AimedPhase(3f));
        }
    }

    void ResetRingBurst() { ringBurstShotsLeft = ringBurstCount; ringBurstCooldown = 0f; }

    IEnumerator RingPhase(float duration) { float t = 0f; while (t < duration && enemyHealth.maxHP > 0) { UpdateRingBurst(Time.deltaTime); t += Time.deltaTime; yield return null; } }
    IEnumerator RingRainPhase(float duration) { float t = 0f, rainTimer = 0f; while (t < duration && enemyHealth.maxHP > 0) { UpdateRingBurst(Time.deltaTime); if (rainTimer <= 0f) { FireRain(); rainTimer = rainFireRate; } rainTimer -= Time.deltaTime; t += Time.deltaTime; yield return null; } }
    IEnumerator AimedPhase(float duration) { float t = 0f; while (t < duration && enemyHealth.maxHP > 0) { FireAimedShot(); yield return new WaitForSeconds(aimedFireRate); t += aimedFireRate; } }

    void UpdateRingBurst(float dt) { if (ringBurstCooldown > 0f) { ringBurstCooldown -= dt; return; } FireRing(); ringBurstShotsLeft--; if (ringBurstShotsLeft <= 0) { ringBurstShotsLeft = ringBurstCount; ringBurstCooldown = ringBurstPause; } else { ringBurstCooldown = ringFireRate; } }

    void FireRing() { ringAngle += ringRotationSpeed; for (int i = 0; i < ringBulletCount; i++) { float angle = ringAngle + (360f / ringBulletCount) * i; SpawnBullet(ringBulletPrefab, angle, ringSpeed); } }
    void FireRain() { float x = transform.position.x + Random.Range(-rainSpread, rainSpread); Vector3 pos = new Vector3(x, transform.position.y, 0f); GameObject b = Instantiate(rainBulletPrefab, pos, Quaternion.identity); EnemyBullet bullet = b.GetComponent<EnemyBullet>(); if (bullet != null) { bullet.direction = Vector2.down; bullet.speed = rainSpeed; } }
    void FireAimedShot() { if (!player) return; Vector2 dir = (player.position - transform.position).normalized; GameObject b = Instantiate(aimedBulletPrefab, transform.position, Quaternion.identity); EnemyBullet bullet = b.GetComponent<EnemyBullet>(); if (bullet != null) { bullet.direction = dir; bullet.speed = aimedSpeed; } }
    void SpawnBullet(GameObject prefab, float angleDeg, float speed) { float rad = angleDeg * Mathf.Deg2Rad; Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)); GameObject b = Instantiate(prefab, transform.position, Quaternion.identity); EnemyBullet bullet = b.GetComponent<EnemyBullet>(); if (bullet != null) { bullet.direction = dir; bullet.speed = speed; } }
    void Update()
    {
        if (GameState.Instance.GameplayLocked) return;

        // movement / shooting logic
    }

}


