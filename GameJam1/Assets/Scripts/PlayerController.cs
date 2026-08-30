using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterAnimation
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 10f;

    [Header("Dash Settings")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1.5f;

    private bool canDash = true;
    public bool isDashing = false;

    [Header("State")]
    public bool isStunned = false;

    private Rigidbody rb;

    private Vector3 moveInput;
    private Vector2 rawInputVector;

    private PlayerInputActions inputActions;

    protected override void Awake()
    {
        base.Awake();

        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = GetComponentInParent<Rigidbody>();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Dash.performed += OnDashPerformed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Dash.performed -= OnDashPerformed;

        inputActions.Player.Disable();
    }

    private void OnMovePerformed(
        InputAction.CallbackContext context)
    {
        rawInputVector = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(
        InputAction.CallbackContext context)
    {
        rawInputVector = Vector2.zero;
    }

    private void OnDashPerformed(
        InputAction.CallbackContext context)
    {
        if (!canDash)
            return;

        if (isStunned)
            return;

        StartCoroutine(PerformDash());
    }

    private void Update()
    {
        if (isStunned)
        {
            moveInput = Vector3.zero;

            SetWalking(false);

            return;
        }

        if (rawInputVector.magnitude < 0.2f)
        {
            rawInputVector = Vector2.zero;
        }

        moveInput =
            new Vector3(
                rawInputVector.x,
                0f,
                rawInputVector.y
            ).normalized;

        if (!isDashing)
        {
            SetWalking(
                moveInput.magnitude > 0.1f
            );
        }
    }

    private void FixedUpdate()
    {
        if (isStunned)
            return;

        if (isDashing)
            return;

        MovePlayer();
    }

    // =========================
    // NORMAL MOVEMENT
    // =========================

    private void MovePlayer()
    {
        if (moveInput.magnitude <= 0.1f)
            return;

        Vector3 targetPosition =
            rb.position +
            moveInput *
            moveSpeed *
            Time.fixedDeltaTime;

        rb.MovePosition(targetPosition);

        Quaternion targetRotation =
            Quaternion.LookRotation(moveInput);

        rb.rotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed *
                Time.fixedDeltaTime
            );
    }

    // =========================
    // DASH
    // =========================

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        SetWalking(false);
        SetDashing(true);

        Vector3 dashDirection =
            moveInput.magnitude > 0.1f
            ? moveInput.normalized
            : transform.forward;

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            if (isStunned)
                break;

            float dashStep =
                dashSpeed *
                Time.fixedDeltaTime;

            RaycastHit hit;

            // شوف هل هنخبط في حاجة
            if (rb.SweepTest(
                dashDirection,
                out hit,
                dashStep,
                QueryTriggerInteraction.Ignore))
            {
                // =========================
                // هل الحاجة Enemy؟
                // =========================

                EnemyKnockout enemyKnockout =
                    hit.collider
                    .GetComponentInParent<EnemyKnockout>();

                if (enemyKnockout != null)
                {
                    Debug.Log(
                        "DASH HIT ENEMY: " +
                        enemyKnockout.gameObject.name
                    );

                    enemyKnockout.TakeKnockout(
                        dashDirection
                    );

                    // اتحرك لحد الـEnemy
                    float moveDistance =
                        Mathf.Max(
                            hit.distance - 0.05f,
                            0f
                        );

                    rb.MovePosition(
                        rb.position +
                        dashDirection *
                        moveDistance
                    );

                    break;
                }

                // =========================
                // مش Enemy = حائط / Shelf
                // =========================

                float safeDistance =
                    Mathf.Max(
                        hit.distance - 0.05f,
                        0f
                    );

                rb.MovePosition(
                    rb.position +
                    dashDirection *
                    safeDistance
                );

                break;
            }

            // مفيش حاجة قدامنا
            rb.MovePosition(
                rb.position +
                dashDirection *
                dashStep
            );

            elapsedTime +=
                Time.fixedDeltaTime;

            yield return
                new WaitForFixedUpdate();
        }

        rb.linearVelocity =
            Vector3.zero;

        isDashing = false;

        SetDashing(false);

        SetWalking(
            moveInput.magnitude > 0.1f
        );

        yield return
            new WaitForSeconds(
                dashCooldown
            );

        canDash = true;
    }

    // =========================
    // EXTRA COLLISION CHECK
    // =========================

    private void OnCollisionEnter(
        Collision collision)
    {
        if (!isDashing)
            return;

        TryKnockoutEnemy(
            collision.collider
        );
    }

    private void OnTriggerEnter(
        Collider other)
    {
        if (!isDashing)
            return;

        TryKnockoutEnemy(other);
    }

    // =========================
    // KNOCKOUT ENEMY
    // =========================

    private void TryKnockoutEnemy(
        Collider hitCollider)
    {
        EnemyKnockout enemyKnockout =
            hitCollider
            .GetComponentInParent<EnemyKnockout>();

        if (enemyKnockout == null)
            return;

        if (enemyKnockout.IsKnockedOut)
            return;

        Debug.Log(
            "PLAYER KNOCKED OUT ENEMY: " +
            enemyKnockout.gameObject.name
        );

        enemyKnockout.TakeKnockout(
            transform.forward
        );
    }
}