using UnityEngine;

public class OffscreenDespawn : MonoBehaviour
{
    public float padding = 1f;

    Camera cam;
    float minX, maxX, minY, maxY;

    void Start()
    {
        cam = Camera.main;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = bottomLeft.x - padding;
        minY = bottomLeft.y - padding;
        maxX = topRight.x + padding;
        maxY = topRight.y + padding;
    }

    void Update()
    {
        Vector3 p = transform.position;

        if (p.x < minX || p.x > maxX || p.y < minY || p.y > maxY)
        {
            Destroy(gameObject);
        }
    }
}
