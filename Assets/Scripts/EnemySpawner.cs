using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public void Spawn()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
