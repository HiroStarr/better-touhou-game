using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialBoss : MonoBehaviour
{
    public Dialogue introDialogue;
    public Dialogue moveDialogue;
    public Dialogue shootDialogue;
    public Dialogue focusDialogue;
    public Dialogue bombDialogue;
    public Dialogue winDialogue;

    public FairyShooter shooter;
    public Enemy enemy;

    [Header("Encircle Bomb Attack")]
    public GameObject encircleBulletPrefab;
    public int encircleCount = 28;
    public float encircleRadius = 4f;
    public float encircleSpeed = 1.6f;

    [Header("Scene Transition")]
    public string nextSceneName = "Stage1";

    void Start()
    {
        GameState.Instance.TutorialMode = true;
        enemy.invincible = true;
        shooter.enabled = false;
        StartCoroutine(TutorialRoutine());
    }

    IEnumerator TutorialRoutine()
    {
        yield return Talk(introDialogue);

        // Movement lesson
        yield return Talk(moveDialogue);
        yield return new WaitForSeconds(4f);

        // Shooting lesson
        yield return Talk(shootDialogue);
        enemy.invincible = false;
        enemy.maxHP = 20;
        yield return new WaitUntil(() => enemy.CurrentHP <= 0);
        enemy.invincible = true;

        // Focus lesson
        yield return Talk(focusDialogue);
        shooter.pattern = FairyShooter.Pattern.Aimed;
        shooter.bulletSpeed = 3.5f;
        shooter.fireRate = 0.8f;
        shooter.enabled = true;
        yield return new WaitForSeconds(8f);
        shooter.enabled = false;

        // Bomb lesson
        yield return Talk(bombDialogue);
        yield return new WaitForSeconds(0.5f);

        shooter.pattern = FairyShooter.Pattern.Aimed;
        shooter.bulletSpeed = 5.5f;
        shooter.fireRate = 0.35f;
        shooter.enabled = true;

        yield return StartCoroutine(EncircleAttack());
        yield return new WaitForSeconds(5f);

        shooter.enabled = false;

        // 🔥 FINAL TEST FIGHT
        yield return StartCoroutine(FinalBossPhase());

        GameState.Instance.TutorialMode = false;
    }

    IEnumerator FinalBossPhase()
    {
        // Boss becomes vulnerable again
        enemy.invincible = false;
        enemy.maxHP = 60;

        // Simple but real patterns
        shooter.pattern = FairyShooter.Pattern.Aimed;
        shooter.bulletSpeed = 4.5f;
        shooter.fireRate = 0.6f;
        shooter.enabled = true;

        // Wait for boss defeat
        yield return new WaitUntil(() => enemy.CurrentHP <= 0);

        shooter.enabled = false;
        enemy.invincible = true;

        yield return Talk(winDialogue);

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator EncircleAttack()
    {
        GameObject hitboxObj = GameObject.FindGameObjectWithTag("PlayerHitbox");
        if (hitboxObj == null)
        {
            Debug.LogError("No PlayerHitbox found!");
            yield break;
        }

        Transform target = hitboxObj.transform;

        for (int i = 0; i < encircleCount; i++)
        {
            float angle = (360f / encircleCount) * i * Mathf.Deg2Rad;
            Vector3 pos = target.position +
                          new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * encircleRadius;

            GameObject b = Instantiate(encircleBulletPrefab, pos, Quaternion.identity);

            EncircleBullet eb = b.GetComponent<EncircleBullet>();
            if (eb != null)
            {
                eb.target = target;
                eb.inwardSpeed = encircleSpeed;
            }
        }

        yield return null;
    }

    IEnumerator Talk(Dialogue d)
    {
        DialogueManager.Instance.ShowDialogue(d);
        while (DialogueManager.Instance.IsDialoguePlaying)
            yield return null;
    }
}
