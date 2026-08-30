using System;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public event Action<Transform> OnPlayerDetected;
    public event Action<Transform> OnPlayerLost;

    private Transform currentPlayer;

    private void OnTriggerEnter(Collider other)
    {
        Transform player = other.transform.root;

        if (!player.CompareTag("Player"))
            return;

        if (currentPlayer != null)
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