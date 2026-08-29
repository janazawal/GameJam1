using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] private float attackRange = 2.5f;

    [Header("Cart Attack")]
    [SerializeField] private float backDistance = 1f;
    [SerializeField] private float maxChargeDistance = 3f;

    [SerializeField] private float backDuration = 0.4f;
    [SerializeField] private float chargeDuration = 0.3f;

    [SerializeField] private float attackCooldown = 2f;

    [Header("Attack Distance")]
    [SerializeField] private float attackStopDistance = 1.3f;

    private EnemyMovement movement;
    private NavMeshAgent agent;

    private Transform target;

    private bool isAttacking;
    private float nextAttackTime;

    public bool IsInCombat => target != null;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (target == null)
            return;

        if (isAttacking)
            return;

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                target.position
            );

        if (distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            movement.Stop();

            TryAttack();
        }
    }

    public void StartCombat(Transform player)
    {
        target = player;

        Debug.Log("Combat Started");
    }

    public void StopCombat()
    {
        if (isAttacking)
            return;

        target = null;

        movement.Stop();

        Debug.Log("Combat Stopped");
    }

    private void ChasePlayer()
    {
        if (target == null)
            return;

        Vector3 direction =
            transform.position -
            target.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        Vector3 stopPosition =
            target.position +
            direction * attackStopDistance;

        movement.Chase(stopPosition);
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime =
            Time.time + attackCooldown;

        StartCoroutine(CartAttack());
    }

    private IEnumerator CartAttack()
    {
        if (target == null)
            yield break;

        isAttacking = true;

        movement.Stop();

        // =========================
        // اتجاه الـPlayer
        // =========================

        Vector3 direction =
            target.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            isAttacking = false;
            yield break;
        }

        direction.Normalize();

        // خلي الـEnemy يبص للـPlayer
        transform.rotation =
            Quaternion.LookRotation(direction);

        // وقفة صغيرة قبل الهجوم
        yield return new WaitForSeconds(0.15f);

        // =========================
        // 1. يرجع لورا
        // =========================

        yield return MoveEnemy(
            -direction,
            backDistance,
            backDuration
        );

        if (target == null)
        {
            isAttacking = false;
            yield break;
        }

        // =========================
        // نحدث اتجاه الـPlayer
        // =========================

        direction =
            target.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            isAttacking = false;
            yield break;
        }

        direction.Normalize();

        transform.rotation =
            Quaternion.LookRotation(direction);

        // =========================
        // 2. نحسب مسافة الـCharge
        // =========================

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                target.position
            );

        float actualChargeDistance =
            distanceToPlayer -
            attackStopDistance;

        actualChargeDistance =
            Mathf.Clamp(
                actualChargeDistance,
                0f,
                maxChargeDistance
            );

        // =========================
        // 3. يندفع بالكارت لقدام
        // =========================

        if (actualChargeDistance > 0f)
        {
            yield return MoveEnemy(
                direction,
                actualChargeDistance,
                chargeDuration
            );
        }

        // =========================
        // 4. نشوف هل وصل للـPlayer
        // =========================

        if (target != null)
        {
            float finalDistance =
                Vector3.Distance(
                    transform.position,
                    target.position
                );

            if (finalDistance <= attackRange)
            {
                PlayerKnockout knockout =
                    target.GetComponent<PlayerKnockout>();

                if (knockout != null &&
                    !knockout.IsKnockedOut)
                {
                    knockout.ApplyKnockout();

                    Debug.Log(
                        "Player hit by cart!"
                    );
                }
            }
        }

        isAttacking = false;
    }

    private IEnumerator MoveEnemy(
        Vector3 direction,
        float distance,
        float duration)
    {
        if (distance <= 0f)
            yield break;

        float timer = 0f;

        float speed =
            distance / duration;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            Vector3 movementStep =
                direction *
                speed *
                Time.deltaTime;

            agent.Move(movementStep);

            yield return null;
        }
    }
}