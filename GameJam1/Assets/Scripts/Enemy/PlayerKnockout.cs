using System;
using System.Collections;
using UnityEngine;

public class PlayerKnockout : MonoBehaviour
{
    [SerializeField] private float knockoutDuration = 5f;

    private bool isKnockedOut;

    public bool IsKnockedOut => isKnockedOut;

    public event Action OnKnockedOut;
    public event Action OnRecovered;

    public void ApplyKnockout()
    {
        if (isKnockedOut)
            return;

        StartCoroutine(KnockoutRoutine());
    }

    private IEnumerator KnockoutRoutine()
    {
        isKnockedOut = true;

        OnKnockedOut?.Invoke();

        Debug.Log("Player Knocked Out");

        yield return new WaitForSeconds(knockoutDuration);

        isKnockedOut = false;

        OnRecovered?.Invoke();

        Debug.Log("Player Recovered");
    }
}