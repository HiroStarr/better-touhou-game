using UnityEngine;

public class EncircleBullet : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public float inwardSpeed = 1.6f;
    public bool rotateTowardTarget = true;

    void Update()
    {
        if (GameState.Instance.GameplayLocked) return;
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * inwardSpeed * Time.deltaTime;

        if (rotateTowardTarget)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }
}
