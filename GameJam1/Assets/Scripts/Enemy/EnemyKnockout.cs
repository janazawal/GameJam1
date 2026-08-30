using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyKnockout : MonoBehaviour
{
    [Header("Knockout")]
    [SerializeField] private float knockoutDuration = 3f;

    [Header("UI")]
    [SerializeField] private KnockoutTimerUI knockoutTimerUI;

    private bool isKnockedOut;

    private NavMeshAgent agent;
    private EnemyMovement movement;
    private EnemyCombat combat;
    private EnemyShopping shopping;
    private EnemyWander wander;
    private EnemyBrain brain;
    private CharacterAnimation characterAnimation;

    public bool IsKnockedOut => isKnockedOut;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
        shopping = GetComponent<EnemyShopping>();
        wander = GetComponent<EnemyWander>();
        brain = GetComponent<EnemyBrain>();
        characterAnimation = GetComponent<CharacterAnimation>();
    }

    public void TakeKnockout(Vector3 hitDirection)
    {
        if (isKnockedOut)
            return;

        StartCoroutine(KnockoutRoutine());
    }

    private IEnumerator KnockoutRoutine()
    {
        // =========================
        // KNOCKOUT START
        // =========================

        isKnockedOut = true;

        Debug.Log("===== ENEMY KNOCKOUT START =====");

        // شغل Knockout Animation + particles
        if (characterAnimation != null)
        {
            characterAnimation.SetKnockedOut(true);
        }

        // وقف الحركة
        if (movement != null)
        {
            movement.Stop();
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // وقف Combat
        if (combat != null)
        {
            combat.SetKnockedOut(true);
            combat.enabled = false;
        }

        // وقف Shopping
        if (shopping != null)
        {
            shopping.PauseShopping();
            shopping.enabled = false;
        }

        // وقف Wandering
        if (wander != null)
        {
            wander.StopWandering();
            wander.enabled = false;
        }

        // وقف Brain
        if (brain != null)
        {
            brain.enabled = false;
        }

        // =========================
        // KNOCKOUT TIMER
        // =========================

        float timer = knockoutDuration;

        while (timer > 0f)
        {
            if (knockoutTimerUI != null)
            {
                knockoutTimerUI.ShowTime(timer);
            }

            timer -= Time.deltaTime;

            yield return null;
        }

        if (knockoutTimerUI != null)
        {
            knockoutTimerUI.Hide();
        }

        // =========================
        // RECOVER
        // =========================

        isKnockedOut = false;

        Debug.Log("===== ENEMY RECOVERING =====");

        // وقف Knockout Animation + particles
        if (characterAnimation != null)
        {
            characterAnimation.SetKnockedOut(false);
        }

        // رجع الـNavMesh
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        // رجع الـBrain
        if (brain != null)
        {
            brain.enabled = true;
        }

        // رجع Combat
        if (combat != null)
        {
            combat.enabled = true;
            combat.SetKnockedOut(false);
        }

        // رجع Shopping
        if (shopping != null)
        {
            shopping.enabled = true;
        }

        // رجع Wander
        if (wander != null)
        {
            wander.enabled = true;
        }

        // =========================
        // RESUME OLD TASK
        // =========================

        if (shopping != null && !shopping.IsShoppingComplete)
        {
            shopping.ResumeShopping();

            Debug.Log("Enemy recovered -> Shopping");
        }
        else if (wander != null)
        {
            wander.StartWandering();

            Debug.Log("Enemy recovered -> Wandering");
        }

        Debug.Log("===== ENEMY KNOCKOUT END =====");
    }
}