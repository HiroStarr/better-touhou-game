[System.Serializable]
public class LevelEvent
{
    public float time;                  // When this wave starts
    public EnemySpawner spawner;
    public int count = 1;
    public float interval = 0.5f;       // Delay between each spawn
}
