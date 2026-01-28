using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    [HideInInspector] public bool GameplayLocked = false;
    [HideInInspector] public bool TutorialMode = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LockGameplay() => GameplayLocked = true;
    public void UnlockGameplay() => GameplayLocked = false;
}
