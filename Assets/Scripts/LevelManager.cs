using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<LevelEvent> events; // Assign in Inspector

    private float timer;
    private int index;

    void Update()
    {
        if (index >= events.Count || events[index] == null) return;

        timer += Time.deltaTime;

        if (timer >= events[index].time)
        {
            StartCoroutine(SpawnWave(events[index]));
            index++;
        }
    }

    IEnumerator SpawnWave(LevelEvent e)
    {
        // Optional: show dialogue
        if (e.dialogue != null)
        {
            DialogueManager.Instance.ShowDialogue(e.dialogue);
            while (DialogueManager.Instance.IsDialoguePlaying)
                yield return null;
        }

        // Spawn enemies
        for (int i = 0; i < e.count; i++)
        {
            if (e.spawner != null)
                e.spawner.Spawn();
            yield return new WaitForSeconds(e.interval);
        }
    }
}
