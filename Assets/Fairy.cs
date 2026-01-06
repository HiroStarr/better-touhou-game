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

    [Header("Stop & Go")]
    public float stopY = 2f;
    public float stopTime = 1.5f;

    [Header("Curve In")]
    public float curveStrength = 3f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float fireRate = 1.5f;

    [Header("Life")]
    public int hp = 3;

    float fireTimer;
    float timer;
    float startX;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
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
        if (transform.position.y > stopY)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
        else if (timer < stopTime)
        {
            timer += Time.deltaTime;
            // pause
        }
        else
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }

    void MoveCurveIn()
    {
        timer += Time.deltaTime;

        float curve = Mathf.Sin(timer) * curveStrength;
        float dirX = -Mathf.Sign(startX) * curve;

        Vector3 dir = new Vector3(dirX, -1f, 0f).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    // ---------------- SHOOTING ----------------
    void HandleShooting()
    {
        if (bulletPrefab == null) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            b.GetComponent<EnemyBullet>().direction = Vector3.down;
        }
    }

    // ---------------- LIFE ----------------
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
            Destroy(gameObject);
    }

    void CheckOffscreen()
    {
        
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
