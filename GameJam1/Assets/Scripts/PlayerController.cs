using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float wallCheckDistance = 1f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

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

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Dash.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Dash.performed -= OnDashPerformed;

        inputActions.Player.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        rawInputVector = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        rawInputVector = Vector2.zero;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (canDash && !isStunned)
        {
            StartCoroutine(PerformDash());
        }
    }

    private void Update()
    {
        if (isStunned)
        {
            moveInput = Vector3.zero;
            return;
        }

        if (rawInputVector.magnitude < 0.2f)
        {
            rawInputVector = Vector2.zero;
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveInput =
            cameraForward * rawInputVector.y +
            cameraRight * rawInputVector.x;

        moveInput.Normalize();
    }
    void FixedUpdate()
    {
        if (isStunned)
            return;

        if (!isDashing)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        if (moveInput.magnitude > 0.1f)
        {
            float currentSpeed = isDashing ? dashSpeed : moveSpeed;

            if (isDashing)
            {
                bool hitWall = Physics.Raycast(
                    transform.position + Vector3.up * 0.5f,
                    moveInput,
                    wallCheckDistance,
                    wallLayer
                );

                if (hitWall)
                {
                    return;
                }
            }

            Vector3 targetPosition =
                rb.position + moveInput * currentSpeed * Time.fixedDeltaTime;

            rb.MovePosition(targetPosition);

            if (rawInputVector.y >= 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveInput);

                rb.rotation = Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }
    }
    IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        Vector3 dashDirection =
            moveInput.magnitude > 0.1f
            ? moveInput.normalized
            : transform.forward;

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            float dashStep = dashSpeed * Time.fixedDeltaTime;

            RaycastHit hit;

            // بيكشف باستخدام جسم الـRigidbody نفسه
            if (rb.SweepTest(
                dashDirection,
                out hit,
                dashStep,
                QueryTriggerInteraction.Ignore))
            {
                // وقف قبل الحيطة بشوية
                float safeDistance = Mathf.Max(hit.distance - 0.05f, 0f);

                rb.MovePosition(
                    rb.position + dashDirection * safeDistance
                );

                break;
            }

            rb.MovePosition(
                rb.position + dashDirection * dashStep
            );

            elapsedTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isDashing)
            return;

        TryKnockoutEnemy(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDashing)
            return;

        TryKnockoutEnemy(other);
    }

    private void TryKnockoutEnemy(Collider hitCollider)
    {
        EnemyKnockout enemyKnockout =
            hitCollider.GetComponentInParent<EnemyKnockout>();

        if (enemyKnockout == null)
        {
            Debug.Log(
                "I HIT: " + hitCollider.name +
                " | NO EnemyKnockout FOUND"
            );

            return;
        }

        Debug.Log(
            "ENEMY FOUND: " +
            enemyKnockout.gameObject.name
        );

        enemyKnockout.TakeKnockout(
            transform.forward
        );

        Debug.Log("KNOCKOUT SENT TO ENEMY");
    }
}