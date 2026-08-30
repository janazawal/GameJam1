using System;
using System.Collections;
using UnityEngine;

public class PlayerKnockout : MonoBehaviour
{
    [Header("Knockout")]
    [SerializeField] private float knockoutDuration = 5f;

    [Header("UI")]
    [SerializeField] private KnockoutTimerUI knockoutTimerUI;

    private bool isKnockedOut;
    private PlayerController playerController;

    public bool IsKnockedOut => isKnockedOut;

    public event Action OnKnockedOut;
    public event Action OnRecovered;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void ApplyKnockout()
    {
        if (isKnockedOut)
            return;

        StartCoroutine(KnockoutRoutine());
    }

    private IEnumerator KnockoutRoutine()
    {
        isKnockedOut = true;

        // وقف حركة و Dash الـPlayer
        if (playerController != null)
        {
            playerController.isStunned = true;
        }

        OnKnockedOut?.Invoke();

        Debug.Log("Player Knocked Out");

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

        isKnockedOut = false;

        // رجع التحكم للـPlayer
        if (playerController != null)
        {
            playerController.isStunned = false;
        }

        OnRecovered?.Invoke();

        Debug.Log("Player Recovered");
    }
}