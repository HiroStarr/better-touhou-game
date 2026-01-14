using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float focusSpeed = 3f;
    public KeyCode focusKey = KeyCode.LeftShift;

    [Header("Animation")]
    public Sprite[] idleFrames;
    public Sprite[] leanLeftFrames;
    public Sprite[] leanRightFrames;
    public float frameRate = 12f;

    [Header("Focus Hitbox")]
    public GameObject hitbox;
    public float hitboxFadeSpeed = 6f;

    [Header("Shooting")]
    public GameObject bulletPrefab;          // unfocused bullets
    public GameObject focusBulletPrefab;     // focused bullets
    public Transform bulletSpawn;
    public float normalFireRate = 10f;
    public float focusFireRate = 6f;

    [Header("Life & Respawn")]
    public int maxLives = 3;
    public Vector3 respawnPosition = new Vector3(0f, -4f, 0f);
    public float invincibilityTime = 2f;

    [Header("Pixel Perfect")]
    public float PPU = 16f;
    public bool snapToPixels = true;

    // ---- private ----
    SpriteRenderer sr;
    SpriteRenderer hitboxSR;

    Sprite[] currentAnim;
    int frame;
    int loopStart;
    float animTimer;
    float fireTimer;

    int currentLives;
    bool isInvincible = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentAnim = idleFrames;

        currentLives = maxLives;

        if (hitbox != null)
        {
            hitboxSR = hitbox.GetComponent<SpriteRenderer>();
            Color c = hitboxSR.color;
            c.a = 0f;
            hitboxSR.color = c;

            hitbox.transform.localScale = Vector3.one;
            hitbox.transform.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        Move();
        Animate();
        FadeHitbox();
        ShootHandler();

        if (snapToPixels)
            SnapToPixels();
    }

    // ---------------- MOVEMENT ----------------
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool focusing = Input.GetKey(focusKey);

        float curSpeed = focusing ? focusSpeed : speed;
        Vector3 move = new Vector3(h, v, 0).normalized;
        transform.position += move * curSpeed * Time.deltaTime;
    }

    // ---------------- ANIMATION ----------------
    void Animate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Sprite[] target =
            h < -0.1f ? leanLeftFrames :
            h > 0.1f ? leanRightFrames :
                        idleFrames;

        int targetLoop = (target == idleFrames) ? 0 : Mathf.Max(0, target.Length - 4);

        if (currentAnim != target)
        {
            currentAnim = target;
            frame = 0;
            loopStart = targetLoop;
            animTimer = 0f;
            sr.sprite = currentAnim[frame];
        }

        animTimer += Time.deltaTime;
        if (animTimer >= 1f / frameRate)
        {
            animTimer = 0f;
            frame++;
            if (frame >= currentAnim.Length)
                frame = loopStart;

            sr.sprite = currentAnim[frame];
        }
    }

    // ---------------- HITBOX ----------------
    void FadeHitbox()
    {
        if (hitboxSR == null) return;

        bool focusing = Input.GetKey(focusKey);
        float targetA = focusing ? 1f : 0f;

        Color c = hitboxSR.color;
        c.a = Mathf.MoveTowards(c.a, targetA, hitboxFadeSpeed * Time.deltaTime);
        hitboxSR.color = c;
    }

    // ---------------- SHOOTING ----------------
    void ShootHandler()
    {
        if (bulletSpawn == null) return;

        bool focusing = Input.GetKey(focusKey);
        float rate = focusing ? focusFireRate : normalFireRate;

        fireTimer += Time.deltaTime;

        if (Input.GetKey(KeyCode.Z) && fireTimer >= 1f / rate)
        {
            fireTimer = 0f;
            Shoot(focusing);
        }
    }

    void Shoot(bool focusing)
    {
        if (focusing && focusBulletPrefab != null)
        {
            float offset = 0.15f;
            Vector3 leftPos = bulletSpawn.position + Vector3.left * offset;
            Vector3 rightPos = bulletSpawn.position + Vector3.right * offset;

            Vector3 leftDir = Vector3.up;
            Vector3 rightDir = Vector3.up;

            // Only home if enemy is in a 60° cone
            Transform target = FindClosestEnemyInCone(60f);
            if (target != null)
            {
                leftDir = (target.position - leftPos).normalized;
                rightDir = (target.position - rightPos).normalized;
            }

            SpawnBullet(focusBulletPrefab, leftPos, leftDir);
            SpawnBullet(focusBulletPrefab, rightPos, rightDir);
        }
        else if (bulletPrefab != null)
        {
            float[] angles = { -20f, -10f, 0f, 10f, 20f };
            foreach (float a in angles)
            {
                Vector3 dir = Quaternion.Euler(0, 0, a) * Vector3.up;
                SpawnBullet(bulletPrefab, bulletSpawn.position, dir);
            }
        }
    }

    void SpawnBullet(GameObject prefab, Vector3 pos, Vector3 dir)
    {
        GameObject b = Instantiate(prefab, pos, Quaternion.identity);
        Bullet1 bullet = b.GetComponent<Bullet1>();
        if (bullet != null)
        {
            bullet.direction = dir;
        }
    }

    // ---------------- TARGETING ----------------
    Transform FindClosestEnemyInCone(float coneAngle)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return null;

        Transform closest = null;
        float minDist = Mathf.Infinity;
        Vector3 pos = transform.position;

        foreach (GameObject e in enemies)
        {
            Vector3 toEnemy = e.transform.position - pos;
            float angle = Vector3.Angle(Vector3.up, toEnemy); // 0° = straight up
            if (angle <= coneAngle / 2f)
            {
                float d = toEnemy.sqrMagnitude;
                if (d < minDist)
                {
                    minDist = d;
                    closest = e.transform;
                }
            }
        }

        return closest;
    }

    // ---------------- DAMAGE & RESPAWN ----------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvincible) return;

        if (other.CompareTag("EnemyBullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    void TakeDamage(int dmg)
    {
        if (isInvincible) return;

        currentLives -= dmg;

        if (currentLives <= 0)
        {
            Die();
        }
        else
        {
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        // Move immediately to respawn position
        transform.position = respawnPosition;

        // Start invincibility blinking
        StartCoroutine(InvincibilityBlink());
    }

    System.Collections.IEnumerator InvincibilityBlink()
    {
        isInvincible = true;
        float timer = 0f;
        float blinkInterval = 0.1f;

        while (timer < invincibilityTime)
        {
            sr.enabled = !sr.enabled;
            if (hitboxSR != null) hitboxSR.enabled = sr.enabled;

            timer += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        sr.enabled = true;
        if (hitboxSR != null) hitboxSR.enabled = true;

        isInvincible = false;
    }

    void Die()
    {
        // Game Over
        gameObject.SetActive(false);
        Debug.Log("Game Over!");
        // Optional: trigger Game Over UI
    }

    // ---------------- PIXEL SNAP ----------------
    void SnapToPixels()
    {
        Vector3 p = transform.position;
        p.x = Mathf.Round(p.x * PPU) / PPU;
        p.y = Mathf.Round(p.y * PPU) / PPU;
        transform.position = p;
    }
}
