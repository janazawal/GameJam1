using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] private float attackRange = 3.5f;
    [SerializeField] private float attackStopDistance = 1.5f;

    [Header("Cart Attack")]
    [SerializeField] private float backDistance = 1.2f;
    [SerializeField] private float chargeDistance = 4f;

    [SerializeField] private float backDuration = 0.4f;
    [SerializeField] private float chargeDuration = 0.45f;

    [SerializeField] private float attackCooldown = 2f;

    [Header("Knockout Hit")]
    [SerializeField] private float knockoutDistance = 2f;

    private EnemyMovement movement;
    private NavMeshAgent agent;
    private EnemyBrain brain;

    private Transform target;

    private bool isAttacking = false;
    private bool isKnockedOut = false;

    private float nextAttackTime = 0f;

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
            target.GetComponentInParent<PlayerKnockout>();

        // لو الـPlayer Knocked Out بالفعل
        // الـEnemy يسيبه ويرجع لمهمته
        if (playerKnockout != null &&
            playerKnockout.IsKnockedOut)
        {
            LeavePlayer();
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                target.position
            );

        if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            if (movement != null)
                movement.Stop();

            TryAttack();
        }
    }

    // =====================================================
    // START COMBAT
    // =====================================================

    public void StartCombat(Transform player)
    {
        if (isKnockedOut)
            return;

        if (player == null)
            return;

        PlayerKnockout playerKnockout =
            player.GetComponentInParent<PlayerKnockout>();

        if (playerKnockout != null &&
            playerKnockout.IsKnockedOut)
        {
            return;
        }

        target = player;

        Debug.Log("ENEMY -> COMBAT START");
    }

    // =====================================================
    // STOP COMBAT
    // =====================================================

    public void StopCombat()
    {
        StopAllCoroutines();

        isAttacking = false;
        target = null;

        if (movement != null)
            movement.Stop();
    }

    // =====================================================
    // LEAVE PLAYER
    // =====================================================

    private void LeavePlayer()
    {
        StopAllCoroutines();

        isAttacking = false;
        target = null;

        if (movement != null)
            movement.Stop();

        if (brain != null)
            brain.LeaveCombatAndResumeTask();
    }

    // =====================================================
    // ENEMY KNOCKOUT STATE
    // =====================================================

    public void SetKnockedOut(bool value)
    {
        isKnockedOut = value;

        if (value)
        {
            StopAllCoroutines();

            isAttacking = false;
            target = null;

            if (movement != null)
                movement.Stop();
        }
    }

    // =====================================================
    // CHASE
    // =====================================================

    private void ChasePlayer()
    {
        if (target == null)
            return;

        if (movement == null)
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

    // =====================================================
    // TRY ATTACK
    // =====================================================

    private void TryAttack()
    {
        if (isKnockedOut)
            return;

        if (isAttacking)
            return;

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime =
            Time.time + attackCooldown;

        StartCoroutine(CartAttack());
    }

    // =====================================================
    // CART ATTACK
    // =====================================================

    private IEnumerator CartAttack()
    {
        if (target == null)
            yield break;

        isAttacking = true;

        Transform player = target;

        if (movement != null)
            movement.Stop();

        // ---------------------------
        // Face Player
        // ---------------------------

        Vector3 direction =
            player.position -
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

        yield return new WaitForSeconds(0.15f);

        if (player == null || isKnockedOut)
        {
            isAttacking = false;
            yield break;
        }

        // =================================================
        // BACK
        // =================================================

        Debug.Log("ENEMY BACK");

        yield return MoveEnemy(
            -direction,
            backDistance,
            backDuration
        );

        if (player == null || isKnockedOut)
        {
            isAttacking = false;
            yield break;
        }

        // ---------------------------
        // Face Player Again
        // ---------------------------

        direction =
            player.position -
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

        yield return new WaitForSeconds(0.1f);

        // =================================================
        // CHARGE
        // =================================================

        Debug.Log("ENEMY CHARGE");

        float elapsed = 0f;

        float chargeSpeed =
            chargeDistance /
            Mathf.Max(chargeDuration, 0.01f);

        while (elapsed < chargeDuration)
        {
            if (player == null || isKnockedOut)
            {
                isAttacking = false;
                yield break;
            }

            // -----------------------------------------
            // كل Frame نشوف هل وصلنا للـPlayer
            // -----------------------------------------

            float distanceToPlayer =
                Vector3.Distance(
                    transform.position,
                    player.position
                );

            if (distanceToPlayer <= knockoutDistance)
            {
                KnockoutPlayer(player);
                yield break;
            }

            // -----------------------------------------
            // اتحرك ناحية Player
            // -----------------------------------------

            Vector3 currentDirection =
                player.position -
                transform.position;

            currentDirection.y = 0f;

            if (currentDirection.sqrMagnitude > 0.01f)
            {
                currentDirection.Normalize();

                transform.rotation =
                    Quaternion.LookRotation(
                        currentDirection
                    );

                Vector3 step =
                    currentDirection *
                    chargeSpeed *
                    Time.deltaTime;

                if (agent != null &&
                    agent.isOnNavMesh)
                {
                    agent.Move(step);
                }
                else
                {
                    transform.position += step;
                }
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        // =================================================
        // FINAL CHECK
        // =================================================

        if (player != null)
        {
            float finalDistance =
                Vector3.Distance(
                    transform.position,
                    player.position
                );

            if (finalDistance <= knockoutDistance)
            {
                KnockoutPlayer(player);
                yield break;
            }
        }

        isAttacking = false;
    }

    // =====================================================
    // KNOCKOUT PLAYER
    // =====================================================

    private void KnockoutPlayer(Transform player)
    {
        if (player == null)
        {
            isAttacking = false;
            return;
        }

        PlayerKnockout playerKnockout =
            player.GetComponentInParent<PlayerKnockout>();

        // لو الـtarget نفسه root
        if (playerKnockout == null)
        {
            playerKnockout =
                player.GetComponent<PlayerKnockout>();
        }

        if (playerKnockout == null)
        {
            Debug.LogError(
                "PLAYER KNOCKOUT SCRIPT NOT FOUND!"
            );

            isAttacking = false;
            return;
        }

        if (playerKnockout.IsKnockedOut)
        {
            isAttacking = false;
            LeavePlayer();
            return;
        }

        Debug.Log("===== PLAYER KNOCKOUT =====");

        playerKnockout.ApplyKnockout();

        // مهم:
        // بعد ما يوقع الـPlayer
        // الـEnemy يبطل يزقه
        isAttacking = false;

        LeavePlayer();
    }

    // =====================================================
    // MANUAL ENEMY MOVEMENT
    // =====================================================

    private IEnumerator MoveEnemy(
        Vector3 direction,
        float distance,
        float duration)
    {
        if (distance <= 0f)
            yield break;

        if (duration <= 0f)
            yield break;

        direction.y = 0f;
        direction.Normalize();

        float elapsed = 0f;
        float speed = distance / duration;

        while (elapsed < duration)
        {
            if (isKnockedOut)
                yield break;

            Vector3 step =
                direction *
                speed *
                Time.deltaTime;

            if (agent != null &&
                agent.isOnNavMesh)
            {
                agent.Move(step);
            }
            else
            {
                transform.position += step;
            }

            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}