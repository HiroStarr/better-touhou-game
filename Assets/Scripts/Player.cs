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

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public GameObject focusBulletPrefab;
    public Transform bulletSpawn;
    public float normalFireRate = 10f;
    public float focusFireRate = 6f;
    public float normalBulletSpeed = 10f;
    public float focusBulletSpeed = 14f;

    [Header("Life & Respawn")]
    public int maxLives = 3;
    public Vector3 respawnPosition = new Vector3(0f, -4f, 0f);
    public float invincibilityTime = 2f;

    [Header("Bomb")]
    public KeyCode bombKey = KeyCode.X;
    public GameObject bombExplosionPrefab;
    public float bombCameraShakeDuration = 0.25f;
    public float bombCameraShakeMagnitude = 0.5f;
    public int bombsPerLife = 3;

    [Header("Deathbomb")]
    public float deathbombWindow = 0.25f;

    [Header("Death Visual")]
    public float deathStretchAmount = 2f;  // Y multiplier
    public float deathSquashAmount = 0.5f; // X multiplier
    public float deathDuration = 0.3f;     // duration of fade/stretch
    public float deathPopHeight = 0.2f;    // upward pop

    [Header("Focus Hitbox")]
    public GameObject hitbox;
    public float hitboxFadeSpeed = 6f;

    [HideInInspector] public bool isInvincible = false;

    // ---- private ----
    private SpriteRenderer sr;
    private SpriteRenderer hitboxSR;
    private Sprite[] currentAnim;
    private int frame;
    private int loopStart;
    private float animTimer;
    private float fireTimer;
    private int currentLives;
    private int currentBombs;
    private bool deathbombActive;
    private bool isDead = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentAnim = idleFrames;
        currentLives = maxLives;
        currentBombs = bombsPerLife;

        if (hitbox != null)
        {
            hitboxSR = hitbox.GetComponent<SpriteRenderer>();
            Color c = hitboxSR.color;
            c.a = 0f;
            hitboxSR.color = c;
        }
    }

    void Update()
    {
        if (GameState.Instance.GameplayLocked) return;
        if (isDead) return;

        Move();
        Animate();
        FadeHitbox();
        ShootHandler();
        BombHandler();
    }

    // ---------------- MOVEMENT ----------------
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float curSpeed = Input.GetKey(focusKey) ? focusSpeed : speed;

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

        float targetA = Input.GetKey(focusKey) ? 1f : 0f;
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
            SpawnBullet(focusBulletPrefab, bulletSpawn.position + Vector3.left * offset, Vector3.up, focusBulletSpeed);
            SpawnBullet(focusBulletPrefab, bulletSpawn.position + Vector3.right * offset, Vector3.up, focusBulletSpeed);
        }
        else
        {
            float[] angles = { -20f, -10f, 0f, 10f, 20f };
            foreach (float a in angles)
            {
                Vector3 dir = Quaternion.Euler(0, 0, a) * Vector3.up;
                SpawnBullet(bulletPrefab, bulletSpawn.position, dir, normalBulletSpeed);
            }
        }
    }

    void SpawnBullet(GameObject prefab, Vector3 pos, Vector3 dir, float speed)
    {
        GameObject b = Instantiate(prefab, pos, Quaternion.identity);
        Bullet1 bullet = b.GetComponent<Bullet1>();
        if (bullet != null)
        {
            bullet.direction = dir;
            bullet.speed = speed;
        }
    }

    // ---------------- DAMAGE ----------------
    public void TakeDamage(int dmg)
    {
        if (isInvincible || deathbombActive || isDead)
            return;

        StartCoroutine(DeathbombRoutine(dmg));
    }

    IEnumerator DeathbombRoutine(int dmg)
    {
        deathbombActive = true;

        if (HitFlash.Instance != null)
            HitFlash.Instance.Flash(0.05f, 0.2f); // short red flash

        float timer = 0f;
        while (timer < deathbombWindow)
        {
            if (Input.GetKeyDown(bombKey) && currentBombs > 0)
            {
                UseBomb();
                deathbombActive = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // auto bomb on death if available
        if (currentBombs > 0)
            UseBomb();

        currentLives -= dmg;

        if (currentLives <= 0)
            Die();
        else
            RespawnPlayer();

        deathbombActive = false;
    }

    // ---------------- BOMB ----------------
    void BombHandler()
    {
        if (Input.GetKeyDown(bombKey) && currentBombs > 0 && !deathbombActive)
            UseBomb();
    }

    void UseBomb()
    {
        currentBombs--;

        if (bombExplosionPrefab != null)
            Instantiate(bombExplosionPrefab, transform.position, Quaternion.identity);

        if (CameraShake.Instance != null)
            CameraShake.Instance.ShakeCamera(bombCameraShakeDuration, bombCameraShakeMagnitude);

        StartCoroutine(InvincibilityBlink());
    }

    IEnumerator InvincibilityBlink()
    {
        isInvincible = true;
        float timer = 0f;
        float blinkInterval = 0.1f;

        while (timer < invincibilityTime)
        {
            sr.enabled = !sr.enabled;
            timer += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        sr.enabled = true;
        isInvincible = false;
    }

    void RespawnPlayer()
    {
        transform.position = respawnPosition;
        currentBombs = bombsPerLife;
        StartCoroutine(InvincibilityBlink());
    }

    // ---------------- DEATH ----------------
    void Die()
    {
        isDead = true;
        StartCoroutine(DeathStretchFade());
    }

    IEnumerator DeathStretchFade()
    {
        isInvincible = true;

        // Reset scale for full effect
        transform.localScale = Vector3.one;
        Vector3 startScale = transform.localScale;
        Vector3 stretchedScale = new Vector3(startScale.x * deathSquashAmount, startScale.y * deathStretchAmount, startScale.z);

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * deathPopHeight;

        Color startColor = sr.color;
        float t = 0f;

        while (t < deathDuration)
        {
            t += Time.deltaTime;
            float p = t / deathDuration;
            float ease = 1f - Mathf.Pow(1f - p, 3f);

            transform.localScale = Vector3.Lerp(startScale, stretchedScale, ease);
            transform.position = Vector3.Lerp(startPos, targetPos, ease);

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, ease);
            sr.color = c;

            yield return null;
        }

        gameObject.SetActive(false);
        Debug.Log("Game Over!");
    }
}
