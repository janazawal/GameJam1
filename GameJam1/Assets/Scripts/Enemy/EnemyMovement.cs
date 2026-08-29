using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    private NavMeshAgent agent;

    private bool trackDestination;

    public event Action OnDestinationReached;

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

        agent.speed = walkSpeed;

        trackDestination = true;

        agent.SetDestination(destination);
    }

    public void Chase(Vector3 destination)
    {
        if (!agent.isOnNavMesh)
            return;

        agent.speed = runSpeed;

        trackDestination = false;

        agent.SetDestination(destination);
    }

    public void Stop()
    {
        if (!agent.isOnNavMesh)
            return;

        agent.ResetPath();

        trackDestination = false;
    }

    private void CheckDestinationReached()
    {
        if (!trackDestination)
            return;

        if (agent.pathPending)
            return;

        if (!agent.hasPath)
            return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            trackDestination = false;

            OnDestinationReached?.Invoke();
        }
    }
}