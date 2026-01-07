using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 velocity;
    public float lifetime = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }
}
