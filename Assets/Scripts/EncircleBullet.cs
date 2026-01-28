using UnityEngine;

public class EncircleBullet : MonoBehaviour
{
    public Transform target;
    public float inwardSpeed = 1.5f;

    void Update()
    {
        if (GameState.Instance.GameplayLocked) return;
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * inwardSpeed * Time.deltaTime;
    }
}
