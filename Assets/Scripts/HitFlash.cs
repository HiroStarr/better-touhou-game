using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitFlash : MonoBehaviour
{
    public static HitFlash Instance;

    [Tooltip("Full-screen red image for flash")]
    public Image flashImage;

    private Coroutine flashRoutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (flashImage != null)
            flashImage.color = new Color(1, 0, 0, 0);
    }

    /// <summary>
    /// Flash red and optionally freeze time
    /// </summary>
    /// <param name="freezeDuration">How long to freeze (seconds)</param>
    /// <param name="fadeDuration">Fade out after freeze (seconds)</param>
    public void Flash(float freezeDuration, float fadeDuration)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(freezeDuration, fadeDuration));
    }

    private IEnumerator FlashRoutine(float freezeDuration, float fadeDuration)
    {
        // Show red overlay immediately
        if (flashImage != null)
            flashImage.color = new Color(1, 0, 0, 0.5f);

        // Freeze time
        Time.timeScale = 0f;

        // Wait in real time for freeze duration
        yield return new WaitForSecondsRealtime(freezeDuration);

        // Resume time
        Time.timeScale = 1f;

        // Fade out over fadeDuration in **real time**
        if (flashImage != null)
        {
            float t = 0f;
            Color startColor = flashImage.color;

            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float p = t / fadeDuration;
                flashImage.color = Color.Lerp(startColor, new Color(1, 0, 0, 0), p);
                yield return null;
            }

            flashImage.color = new Color(1, 0, 0, 0);
        }
    }
}
