using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : CharacterAnimation
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    private NavMeshAgent agent;

    private bool trackDestination;

    public event Action OnDestinationReached;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        CheckDestinationReached();
        if (agent != null)
        {
            SetWalking(agent.velocity.magnitude > 0.1f);
        }
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