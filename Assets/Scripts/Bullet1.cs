using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    public float speed = 10f;
    public Vector3 direction = Vector3.up;

    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;

        if (Mathf.Abs(transform.position.y) > 20f ||
            Mathf.Abs(transform.position.x) > 20f)
            Destroy(gameObject);
    }
}
