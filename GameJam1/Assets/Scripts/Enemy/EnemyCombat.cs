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
    private EnemyBrain brain;

    private Transform target;

    private bool isAttacking;
    private bool isKnockedOut;

    private float nextAttackTime;

    public bool IsInCombat => target != null;
    public bool IsKnockedOut => isKnockedOut;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        agent = GetComponent<NavMeshAgent>();
        brain = GetComponent<EnemyBrain>();
    }

    private void Update()
    {
        if (isKnockedOut)
            return;

        if (target == null)
            return;

        if (isAttacking)
            return;

        PlayerKnockout playerKnockout =
            target.GetComponent<PlayerKnockout>();

        // لو الـPlayer already knocked out
        // سيبه ومتكملش ضرب
        if (playerKnockout != null &&
            playerKnockout.IsKnockedOut)
        {
            LeavePlayer();
            return;
        }

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

    // =========================
    // START COMBAT
    // =========================

    public void StartCombat(Transform player)
    {
        if (isKnockedOut)
            return;

        target = player;

        Debug.Log("Combat Started");
    }

    // =========================
    // STOP COMBAT
    // =========================

    public void StopCombat()
    {
        StopAllCoroutines();

        isAttacking = false;

        target = null;

        if (movement != null)
        {
            movement.Stop();
        }

        Debug.Log("Combat Stopped");
    }

    // =========================
    // LEAVE PLAYER
    // =========================

    private void LeavePlayer()
    {
        StopAllCoroutines();

        isAttacking = false;

        target = null;

        if (movement != null)
        {
            movement.Stop();
        }

        if (brain != null)
        {
            brain.LeaveCombatAndResumeTask();
        }
    }

    // =========================
    // KNOCKOUT
    // =========================

    public void SetKnockedOut(bool value)
    {
        isKnockedOut = value;

        if (isKnockedOut)
        {
            StopAllCoroutines();

            isAttacking = false;

            if (movement != null)
            {
                movement.Stop();
            }

            Debug.Log("Enemy Combat Disabled");
        }
        else
        {
            Debug.Log("Enemy Combat Enabled");
        }
    }

    // =========================
    // CHASE
    // =========================

    private void ChasePlayer()
    {
        if (target == null)
            return;

        if (isKnockedOut)
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

    // =========================
    // TRY ATTACK
    // =========================

    private void TryAttack()
    {
        if (isKnockedOut)
            return;

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime =
            Time.time + attackCooldown;

        StartCoroutine(CartAttack());
    }

    // =========================
    // CART ATTACK
    // =========================

    private IEnumerator CartAttack()
    {
        if (target == null)
            yield break;

        if (isKnockedOut)
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

        transform.rotation =
            Quaternion.LookRotation(direction);

        // وقفة صغيرة قبل الهجوم
        yield return new WaitForSeconds(0.15f);

        if (isKnockedOut)
        {
            isAttacking = false;
            yield break;
        }

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

        if (isKnockedOut)
        {
            isAttacking = false;
            yield break;
        }

        // =========================
        // تحديث اتجاه الـPlayer
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
        // حساب مسافة الـCharge
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
        // 2. Charge
        // =========================

        if (actualChargeDistance > 0f)
        {
            yield return MoveEnemy(
                direction,
                actualChargeDistance,
                chargeDuration
            );
        }

        if (isKnockedOut)
        {
            isAttacking = false;
            yield break;
        }

        // =========================
        // 3. Hit Player
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

                    Debug.Log("Player hit by cart!");

                    isAttacking = false;

                    // خلاص ضربه، سيبه وامشي
                    LeavePlayer();

                    yield break;
                }
            }
        }

        isAttacking = false;
    }

    // =========================
    // MANUAL MOVEMENT
    // =========================

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
            if (isKnockedOut)
                yield break;

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