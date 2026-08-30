using System;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public event Action<Transform> OnPlayerDetected;
    public event Action<Transform> OnPlayerLost;

    private Transform currentPlayer;

    private void OnTriggerEnter(Collider other)
    {
        TryDetectPlayer(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDetectPlayer(other);
    }

    private void TryDetectPlayer(Collider other)
    {
        Transform player = other.transform.root;

        if (!player.CompareTag("Player"))
            return;

        PlayerKnockout knockout =
            player.GetComponent<PlayerKnockout>();

        // لو الـPlayer Knocked Out متدخلش Combat
        if (knockout != null && knockout.IsKnockedOut)
        {
            if (currentPlayer == player)
            {
                Transform lostPlayer = currentPlayer;

                currentPlayer = null;

                OnPlayerLost?.Invoke(lostPlayer);

                Debug.Log("PLAYER KNOCKED OUT -> ENEMY LEAVES COMBAT");
            }

            return;
        }

        // already detected
        if (currentPlayer == player)
            return;

        currentPlayer = player;

        Debug.Log("PLAYER DETECTED");

        OnPlayerDetected?.Invoke(currentPlayer);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform player = other.transform.root;

        if (!player.CompareTag("Player"))
            return;

        if (player != currentPlayer)
            return;

        Debug.Log("PLAYER LOST");

        Transform lostPlayer = currentPlayer;

        currentPlayer = null;

        OnPlayerLost?.Invoke(lostPlayer);
    }
}