using UnityEngine;

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
    public GameObject bulletPrefab;          // unfocused bullet
    public GameObject focusBulletPrefab;     // focused bullet (different sprite)
    public Transform bulletSpawn;
    public float normalFireRate = 10f;
    public float focusFireRate = 6f;

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

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentAnim = idleFrames;

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

            SpawnBullet(focusBulletPrefab, bulletSpawn.position + Vector3.left * offset, Vector3.up);
            SpawnBullet(focusBulletPrefab, bulletSpawn.position + Vector3.right * offset, Vector3.up);
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
        b.GetComponent<Bullet>().direction = dir;
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
