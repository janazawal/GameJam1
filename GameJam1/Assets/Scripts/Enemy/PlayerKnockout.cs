using System;
using System.Collections;
using UnityEngine;

public class PlayerKnockout : MonoBehaviour
{
    [SerializeField] private float knockoutDuration = 5f;
    [SerializeField] private KnockoutTimerUI knockoutTimerUI;

    private PlayerController playerController;

    public bool IsKnockedOut { get; private set; }

    public event Action OnKnockedOut;
    public event Action OnRecovered;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        if (playerController == null)
        {
            playerController = GetComponentInChildren<PlayerController>();
        }
    }

    public void ApplyKnockout()
    {
        Debug.Log("APPLY KNOCKOUT CALLED");

        if (IsKnockedOut)
            return;

        StartCoroutine(KnockoutRoutine());
    }

    private IEnumerator KnockoutRoutine()
    {
        IsKnockedOut = true;

        Debug.Log("PLAYER IS NOW KNOCKED OUT");

        if (playerController != null)
        {
            playerController.isStunned = true;

            Debug.Log("PLAYER STUNNED = TRUE");
        }
        else
        {
            Debug.LogError("PLAYER CONTROLLER NOT FOUND");
        }

        OnKnockedOut?.Invoke();

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

        IsKnockedOut = false;

        if (playerController != null)
        {
            playerController.isStunned = false;

            Debug.Log("PLAYER STUNNED = FALSE");
        }

        OnRecovered?.Invoke();

        Debug.Log("PLAYER RECOVERED");
    }
}