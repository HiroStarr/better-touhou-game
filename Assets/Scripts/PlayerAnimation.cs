using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Sprite[] idleFrames;
    public Sprite[] leanLeftFrames;
    public Sprite[] leanRightFrames;

    public float frameRate = 12f;

    private SpriteRenderer sr;
    private float timer;
    private int currentFrame;
    private Sprite[] currentAnimation;
    private int loopStartIndex;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        SetAnimation(idleFrames, 0);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrame++;
            if (currentFrame >= currentAnimation.Length)
                currentFrame = loopStartIndex; // loop from the correct frame

            sr.sprite = currentAnimation[currentFrame];
        }
    }

    public void SetLean(float x)
    {
        if (x < -0.1f)
            SetAnimation(leanLeftFrames, Mathf.Max(0, leanLeftFrames.Length - 4));
        else if (x > 0.1f)
            SetAnimation(leanRightFrames, Mathf.Max(0, leanRightFrames.Length - 4));
        else
            SetAnimation(idleFrames, 0);
    }

    void SetAnimation(Sprite[] animFrames, int startLoopIndex)
    {
        if (currentAnimation == animFrames) return;

        currentAnimation = animFrames;
        loopStartIndex = startLoopIndex;
        currentFrame = 0;
        timer = 0f;
        sr.sprite = currentAnimation[currentFrame];
    }
}
