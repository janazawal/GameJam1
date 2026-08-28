using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    public event Action OnDestinationReached;

    private bool isMoving;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        CheckDestinationReached();
    }

    public void MoveTo(Vector3 destination)
    {
        if (!agent.isOnNavMesh)
            return;

        agent.SetDestination(destination);
        isMoving = true;
    }

    public void Stop()
    {
        if (!agent.isOnNavMesh)
            return;

        agent.ResetPath();
        isMoving = false;
    }

    private void CheckDestinationReached()
    {
        if (!isMoving)
            return;

        if (agent.pathPending)
            return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            isMoving = false;
            OnDestinationReached?.Invoke();
        }
    }
}