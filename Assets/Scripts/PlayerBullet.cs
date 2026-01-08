using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        BossAttack boss = other.GetComponent<BossAttack>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
