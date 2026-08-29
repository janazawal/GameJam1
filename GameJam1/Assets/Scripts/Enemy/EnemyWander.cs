using System.Collections;
using UnityEngine;

public class EnemyWander : MonoBehaviour
{
    [SerializeField] private Transform[] wanderPoints;

    [Header("Waiting")]
    [SerializeField] private float minWaitTime = 0.5f;
    [SerializeField] private float maxWaitTime = 1.5f;

    private EnemyMovement movement;

    private int currentPointIndex = 0;

    private bool isWandering = false;
    private bool isWaiting = false;

    public bool IsWandering => isWandering;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    private void OnEnable()
    {
        if (movement != null)
        {
            movement.OnDestinationReached += HandleDestinationReached;
        }
    }

    private void OnDisable()
    {
        if (movement != null)
        {
            movement.OnDestinationReached -= HandleDestinationReached;
        }
    }

    public void SetWanderPoints(Transform[] points)
    {
        wanderPoints = points;
    }

    public void StartWandering()
    {
        if (isWandering)
            return;

        if (wanderPoints == null || wanderPoints.Length == 0)
        {
            Debug.LogWarning("No Wander Points assigned");
            return;
        }

        isWandering = true;
        isWaiting = false;

        GoToCurrentPoint();
    }

    public void StopWandering()
    {
        isWandering = false;
        isWaiting = false;

        StopAllCoroutines();

        if (movement != null)
        {
            movement.Stop();
        }
    }

    private void HandleDestinationReached()
    {
        if (!isWandering)
            return;

        if (isWaiting)
            return;

        StartCoroutine(WaitThenMove());
    }

    private IEnumerator WaitThenMove()
    {
        isWaiting = true;

        float waitTime =
            Random.Range(
                minWaitTime,
                maxWaitTime
            );

        yield return new WaitForSeconds(waitTime);

        if (!isWandering)
        {
            isWaiting = false;
            yield break;
        }

        currentPointIndex++;

        if (currentPointIndex >= wanderPoints.Length)
        {
            currentPointIndex = 0;
        }

        isWaiting = false;

        GoToCurrentPoint();
    }

    private void GoToCurrentPoint()
    {
        if (!isWandering)
            return;

        if (wanderPoints == null || wanderPoints.Length == 0)
            return;

        Transform targetPoint =
            wanderPoints[currentPointIndex];

        if (targetPoint == null)
        {
            Debug.LogWarning(
                "One of the Wander Points is missing"
            );

            return;
        }

        movement.MoveTo(
            targetPoint.position
        );
    }
}