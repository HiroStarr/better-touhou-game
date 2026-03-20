using UnityEngine;

public class PlayerBoundsClamp : MonoBehaviour
{
    [SerializeField] private BoxCollider2D playArea; // Assign your invisible PlayArea collider
    [SerializeField] private float padding = 0.05f;  // Small extra margin

    private Collider2D playerCollider;
    private SpriteRenderer playerSprite;

    void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
        playerSprite = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (playArea == null) return;

        Bounds areaBounds = playArea.bounds;
        Vector3 pos = transform.position;

        Vector2 extents = GetPlayerExtents();

        pos.x = Mathf.Clamp(
            pos.x,
            areaBounds.min.x + extents.x + padding,
            areaBounds.max.x - extents.x - padding
        );

        pos.y = Mathf.Clamp(
            pos.y,
            areaBounds.min.y + extents.y + padding,
            areaBounds.max.y - extents.y - padding
        );

        transform.position = pos;
    }

    Vector2 GetPlayerExtents()
    {
        if (playerCollider != null)
            return playerCollider.bounds.extents;

        if (playerSprite != null)
            return playerSprite.bounds.extents;

        return Vector2.zero;
    }
}