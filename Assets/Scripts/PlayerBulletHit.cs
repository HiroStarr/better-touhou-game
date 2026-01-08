using UnityEngine;

public class PlayerBulletHit : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        Fairy fairy = other.GetComponent<Fairy>();
        if (fairy != null)
        {
            fairy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
