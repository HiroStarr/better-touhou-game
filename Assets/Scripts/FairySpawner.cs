using UnityEngine;

public class FairySpawner : MonoBehaviour
{
    public GameObject fairyPrefab;
    public float spawnInterval = 1.2f;
    public float spawnXRange = 4f;
    public float spawnY = 6f;

    float timer;

    void Update()
    {
        if (fairyPrefab == null) return;

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

        // Pick a random movement pattern
        Fairy.MovementPattern pattern =
            (Fairy.MovementPattern)Random.Range(0, 4);

        f.GetComponent<Fairy>().pattern = pattern;
    }
}
