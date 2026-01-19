using UnityEngine;

public class Fairy : MonoBehaviour
{
    public enum MovementPattern
    {
        StraightDown,
        SineWave,
        StopAndGo,
        CurveIn
    }

    [Header("Movement")]
    public MovementPattern pattern = MovementPattern.StraightDown;
    public float speed = 2f;

    [Header("Sine Wave")]
    public float sineAmplitude = 1.5f;
    public float sineFrequency = 2f;

    [Header("Stop & Go (Time Based)")]
    public float moveTime = 1.5f;   // Move before stopping
    public float stopTime = 1.5f;   // Time spent stopped

    [Header("Curve In")]
    public float curveStrength = 2f;

    [Header("Life")]
    public int hp = 3;

    // ---------------- PRIVATE ----------------

    float timer;
    float startX;

    // Stop & Go state
    float stopGoTimer;
    bool isStopped;
    bool resumed;

    Camera cam;

    void Start()
    {
        startX = transform.position.x;
        cam = Camera.main;
    }

    void OnEnable()
    {
        // Safety reset (important for pooling)
        timer = 0f;
        stopGoTimer = 0f;
        isStopped = false;
        resumed = false;
    }

    void Update()
    {
        HandleMovement();
        CheckOffscreen();
    }

    // ---------------- MOVEMENT ----------------

    void HandleMovement()
    {
        switch (pattern)
        {
            case MovementPattern.StraightDown:
                MoveStraight();
                break;

            case MovementPattern.SineWave:
                MoveSine();
                break;

            case MovementPattern.StopAndGo:
                MoveStopAndGo();
                break;

            case MovementPattern.CurveIn:
                MoveCurveIn();
                break;
        }
    }

    void MoveStraight()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    void MoveSine()
    {
        timer += Time.deltaTime;

        float x = startX + Mathf.Sin(timer * sineFrequency) * sineAmplitude;
        float y = transform.position.y - speed * Time.deltaTime;

        transform.position = new Vector3(x, y, 0f);
    }

    void MoveStopAndGo()
    {
        stopGoTimer += Time.deltaTime;

        // Phase 1: Move
        if (!isStopped)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;

            if (stopGoTimer >= moveTime)
            {
                isStopped = true;
                stopGoTimer = 0f;
            }
            return;
        }

        // Phase 2: Stop
        if (!resumed)
        {
            if (stopGoTimer >= stopTime)
            {
                resumed = true;
                stopGoTimer = 0f;
            }
            return;
        }

        // Phase 3: Resume moving
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    void MoveCurveIn()
    {
        timer += Time.deltaTime;

        float curve = Mathf.Sin(timer) * curveStrength;
        float dirX = -Mathf.Sign(startX) * curve;

        Vector3 dir = new Vector3(dirX, -1f, 0f).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    // ---------------- LIFE ----------------

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
            Destroy(gameObject);
    }

    // ---------------- CLEANUP ----------------

    void CheckOffscreen()
    {
        if (!cam) return;

        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
        if (viewPos.y < -0.1f || viewPos.x < -0.2f || viewPos.x > 1.2f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }
}
