using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 velocity;
    public float lifetime = 10f;

    public Vector3 direction { get; internal set; }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }
}
