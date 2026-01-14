using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 4f;
    public Vector2 direction;

    void Update()
    {
        transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
