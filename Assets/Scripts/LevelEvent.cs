using UnityEngine;

[System.Serializable]  // Important! Makes it show in Inspector
public class LevelEvent
{
    public float time; // Time after level start
    public int count; // Number of enemies
    public float interval = 0.1f; // Between spawns
    public EnemySpawner spawner; // Reference to EnemySpawner
    public Dialogue dialogue; // Optional: dialogue to show
}
