using UnityEngine;

public class Fairy : MonoBehaviour
{
    public enum MovementPattern { StraightDown, SineWave, StopAndGo, CurveIn }

    public MovementPattern pattern = MovementPattern.StraightDown;
    public float speed = 2f;
    public float sineAmplitude = 1.5f;
    public float sineFrequency = 2f;
    public float moveTime = 1.5f;
    public float stopTime = 1.5f;
    public float curveStrength = 2f;

    private float timer, startX, stopGoTimer;
    private bool isStopped, resumed;
    private Camera cam;

    void Start()
    {
        startX = transform.position.x;
        cam = Camera.main;
    }

    void OnEnable() { timer = 0f; stopGoTimer = 0f; isStopped = false; resumed = false; }

    void Update()
    {
        HandleMovement();
        CheckOffscreen();
    }

    void HandleMovement()
    {
        switch (pattern)
        {
            case MovementPattern.StraightDown: transform.position += Vector3.down * speed * Time.deltaTime; break;
            case MovementPattern.SineWave:
                timer += Time.deltaTime;
                transform.position = new Vector3(startX + Mathf.Sin(timer * sineFrequency) * sineAmplitude,
                    transform.position.y - speed * Time.deltaTime, 0f); break;
            case MovementPattern.StopAndGo:
                stopGoTimer += Time.deltaTime;
                if (!isStopped) { transform.position += Vector3.down * speed * Time.deltaTime; if (stopGoTimer >= moveTime) { isStopped = true; stopGoTimer = 0f; } return; }
                if (!resumed) { if (stopGoTimer >= stopTime) { resumed = true; stopGoTimer = 0f; } return; }
                transform.position += Vector3.down * speed * Time.deltaTime; break;
            case MovementPattern.CurveIn:
                timer += Time.deltaTime;
                float curve = Mathf.Sin(timer) * curveStrength;
                float dirX = -Mathf.Sign(startX) * curve;
                transform.position += new Vector3(dirX, -1f, 0f).normalized * speed * Time.deltaTime; break;
        }
    }

    void CheckOffscreen()
    {
        if (!cam) return;
        Vector3 v = cam.WorldToViewportPoint(transform.position);
        if (v.y < -0.1f || v.x < -0.2f || v.x > 1.2f) Destroy(gameObject);
    }
}
