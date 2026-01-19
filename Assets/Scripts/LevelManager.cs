using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<LevelEvent> events;

    float timer;
    int index;

    void Update()
    {
        if (index >= events.Count) return;

        timer += Time.deltaTime;

        if (timer >= events[index].time)
        {
            StartCoroutine(SpawnWave(events[index]));
            index++;
        }
    }

    IEnumerator SpawnWave(LevelEvent e)
    {
        for (int i = 0; i < e.count; i++)
        {
            e.spawner.Spawn();
            yield return new WaitForSeconds(e.interval);
        }
    }
}
