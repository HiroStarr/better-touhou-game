using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject panel;     // The dialogue panel
    public Text dialogueText;    // Text component inside the panel

    [Header("Settings")]
    public float lettersPerSecond = 40f;

    [HideInInspector] public bool IsDialoguePlaying { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (panel != null) panel.SetActive(false);
    }

    // Call this to start a dialogue
    public void ShowDialogue(Dialogue dialogue)
    {
        if (IsDialoguePlaying) return;
        StartCoroutine(RunDialogue(dialogue));
    }

    private IEnumerator RunDialogue(Dialogue dialogue)
    {
        IsDialoguePlaying = true;
        panel.SetActive(true);

        foreach (string line in dialogue.lines)
        {
            dialogueText.text = "";
            foreach (char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(1f / lettersPerSecond);
            }

            // Wait for player to press a key to continue
            while (!Input.GetKeyDown(KeyCode.Z))
                yield return null;
        }

        panel.SetActive(false);
        IsDialoguePlaying = false;
    }
}
