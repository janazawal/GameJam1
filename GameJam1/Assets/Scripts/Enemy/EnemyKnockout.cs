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

    public bool IsKnockedOut => isKnockedOut;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
        shopping = GetComponent<EnemyShopping>();
        wander = GetComponent<EnemyWander>();
        brain = GetComponent<EnemyBrain>();
    }

    public void TakeKnockout(Vector3 hitDirection)
    {
        Debug.Log(
            "TakeKnockout CALLED on " +
            gameObject.name +
            " | already KO = " +
            isKnockedOut
        );

        if (isKnockedOut)
            return;

        StartCoroutine(KnockoutRoutine());
    }

    private IEnumerator KnockoutRoutine()
    {
        Debug.Log("===== ENEMY KNOCKOUT START =====");

        isKnockedOut = true;

        // وقف الحركة فوراً
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

        // وقف الـAI
        if (combat != null)
        {
            combat.SetKnockedOut(true);
            combat.enabled = false;
        }

        if (shopping != null)
        {
            shopping.PauseShopping();
            shopping.enabled = false;
        }

        if (wander != null)
        {
            wander.StopWandering();
            wander.enabled = false;
        }

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

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        if (brain != null)
        {
            brain.enabled = true;
        }

        if (combat != null)
        {
            combat.enabled = true;
            combat.SetKnockedOut(false);
        }

        if (shopping != null)
        {
            shopping.enabled = true;
        }

        if (wander != null)
        {
            wander.enabled = true;
        }

        // رجعه يكمل الـShopping
        // ولو خلص Shopping خليه Wander
        if (shopping != null &&
            !shopping.IsShoppingComplete)
        {
            shopping.ResumeShopping();

            Debug.Log(
                "Enemy recovered -> Shopping"
            );
        }
        else if (wander != null)
        {
            wander.StartWandering();

            Debug.Log(
                "Enemy recovered -> Wandering"
            );
        }

        Debug.Log("===== ENEMY KNOCKOUT END =====");
    }
}