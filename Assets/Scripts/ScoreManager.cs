using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int points = 0;
    public TextMeshProUGUI pointsText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        UpdateUI();
    }

    /// <summary>
    /// Add points
    /// </summary>
    public void AddPoints(int amount)
    {
        points += amount;
        UpdateUI();
    }

    /// <summary>
    /// Update the points display
    /// </summary>
    void UpdateUI()
    {
        if (pointsText != null)
            pointsText.text = "Score: " + points.ToString();
    }

    /// <summary>
    /// Reset points
    /// </summary>
    public void ResetPoints()
    {
        points = 0;
        UpdateUI();
    }
}
