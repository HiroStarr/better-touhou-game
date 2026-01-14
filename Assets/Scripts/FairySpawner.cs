using UnityEngine;

public class FairySpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject fairyPrefab;
    public float spawnInterval = 1.2f;
    public float spawnXRange = 4f;
    public float spawnY = 6f;

    float timer;

    void Update()
    {
        if (!fairyPrefab) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnFairy();
        }
    }

    void SpawnFairy()
    {
        float x = Random.Range(-spawnXRange, spawnXRange);
        Vector3 pos = new Vector3(x, spawnY, 0f);

        GameObject f = Instantiate(fairyPrefab, pos, Quaternion.identity);

        // -------- Movement Pattern --------
        Fairy fairy = f.GetComponent<Fairy>();
        if (fairy)
        {
            fairy.pattern = (Fairy.MovementPattern)
                Random.Range(0, System.Enum.GetValues(typeof(Fairy.MovementPattern)).Length);
        }

        // -------- Bullet Pattern --------
        FairyShooter shooter = f.GetComponent<FairyShooter>();
        if (shooter)
        {
            shooter.pattern = (FairyShooter.Pattern)
                Random.Range(0, System.Enum.GetValues(typeof(FairyShooter.Pattern)).Length);
        }
    }
}
