using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class PickUP : MonoBehaviour
{
    [SerializeField] private Items item;
    [SerializeField] private string playerTag = "Player";

    private bool playerInRange;
    private Pocket playerPocket;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.Interact.performed -= OnInteractPerformed;
        inputActions.Player.Disable();
    }

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger: " + other.name + " | Tag: " + other.tag);

        if (!other.CompareTag(playerTag)) return;

        playerPocket = other.GetComponentInParent<Pocket>();
        Debug.Log("Pocket found: " + (playerPocket != null));

        if (playerPocket != null)
        {
            Debug.Log("In Range");
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInRange = false;
        playerPocket = null;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!playerInRange || playerPocket == null) return;

        bool added = playerPocket.AddItem(item);
        if (added)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Could not pick up " + item + " (not tracked or pocket full)");
        }
    }
}