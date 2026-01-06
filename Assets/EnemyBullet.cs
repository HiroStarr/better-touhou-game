using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 4f;
    public Vector3 direction = Vector3.down;

    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;

       
    }
}
