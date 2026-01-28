using System.Collections;
using UnityEngine;

public class TutorialBoss : MonoBehaviour
{
    public Dialogue introDialogue;
    public Dialogue moveDialogue;
    public Dialogue shootDialogue;
    public Dialogue focusDialogue;
    public Dialogue bombDialogue;
    public Dialogue endDialogue;

    public FairyShooter shooter;
    public Enemy enemy;

    [Header("Bomb Cage")]
    public GameObject encircleBulletPrefab;
    public int cageBulletCount = 32;
    public float cageRadius = 4f;
    public float cageInwardSpeed = 1.2f;

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
        shooter.pattern = FairyShooter.Pattern.Circle;
        shooter.bulletSpeed = 1.5f;
        shooter.fireRate = 2f;
        shooter.enabled = true;
        yield return new WaitForSeconds(8f);
        shooter.enabled = false;

        // Bomb lesson
        yield return Talk(bombDialogue);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(BombCageAttack());
        yield return new WaitForSeconds(4f);

        yield return Talk(endDialogue);

        GameState.Instance.TutorialMode = false;
    }

    IEnumerator BombCageAttack()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null) yield break;

        for (int i = 0; i < cageBulletCount; i++)
        {
            float angle = (360f / cageBulletCount) * i * Mathf.Deg2Rad;
            Vector3 pos = player.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * cageRadius;

            GameObject b = Instantiate(encircleBulletPrefab, pos, Quaternion.identity);
            EncircleBullet eb = b.GetComponent<EncircleBullet>();
            if (eb != null)
            {
                eb.target = player;
                eb.inwardSpeed = cageInwardSpeed;
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
