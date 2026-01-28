using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public float lettersPerSecond = 40f;

    public bool IsDialoguePlaying { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (panel != null) panel.SetActive(false);
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        if (IsDialoguePlaying) return;
        StartCoroutine(RunDialogue(dialogue));
    }

    private IEnumerator RunDialogue(Dialogue dialogue)
    {
        IsDialoguePlaying = true;
        panel.SetActive(true);

        // 🔒 Lock gameplay during dialogue
        if (GameState.Instance != null)
            GameState.Instance.LockGameplay();

        foreach (string line in dialogue.lines)
        {
            dialogueText.text = "";

            foreach (char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(1f / lettersPerSecond);
            }

            while (!Input.GetKeyDown(KeyCode.Z))
                yield return null;
        }

        panel.SetActive(false);
        IsDialoguePlaying = false;

        // 🔓 Unlock gameplay after dialogue
        if (GameState.Instance != null)
            GameState.Instance.UnlockGameplay();
    }
}
