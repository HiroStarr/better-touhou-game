using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    public float focusSpeed = 3f;
    public GameObject hitbox;
    public float fadeSpeed = 6f;

    private PlayerAnimation anim;
    private SpriteRenderer hitboxSR;

    void Awake()
    {
        anim = GetComponent<PlayerAnimation>();

        if (hitbox != null)
        {
            hitboxSR = hitbox.GetComponent<SpriteRenderer>();
            Color c = hitboxSR.color;
            c.a = 0f;
            hitboxSR.color = c;
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool focusing = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = focusing ? focusSpeed : speed;

        // Move player
        Vector3 move = new Vector3(h, v, 0).normalized;
        transform.position += move * currentSpeed * Time.deltaTime;

        // Lean animations
        anim.SetLean(h);

        // Hitbox fade
        if (hitboxSR != null)
        {
            Color c = hitboxSR.color;
            float targetAlpha = focusing ? 1f : 0f;
            c.a = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
            hitboxSR.color = c;
        }
    }
}
