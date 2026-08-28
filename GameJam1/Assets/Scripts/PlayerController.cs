using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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
    public bool isStunned = false;//when the player is knockedout

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
    
    void Start()
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

    void Update()
    {
        if (isStunned)
        {
            moveInput = Vector3.zero;
            return;
        }

        moveInput = new Vector3(rawInputVector.x, 0f, rawInputVector.y).normalized;
    }
    void FixedUpdate()
    {
        if (isStunned) return;

        MovePlayer();
    }

    void MovePlayer()
    {
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 targetPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);

            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        Vector3 dashDirection = moveInput.magnitude > 0.1f ? moveInput : transform.forward;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            Vector3 targetDashPos = rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetDashPos);

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
        if (isDashing && collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.SendMessage("TakeKnockout", transform.forward, SendMessageOptions.DontRequireReceiver);
        }
    }
}
