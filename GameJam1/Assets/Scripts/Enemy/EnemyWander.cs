using System.Collections;
using UnityEngine;

public class EnemyWander : MonoBehaviour
{
    [SerializeField] private Transform[] wanderPoints;

    [Header("Waiting")]
    [SerializeField] private float minWaitTime = 0.5f;
    [SerializeField] private float maxWaitTime = 1.5f;

    private EnemyMovement movement;

    private bool isWandering = false;
    private bool isWaiting = false;

    private int lastPointIndex = -1;

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

        GoToRandomPoint();
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

        isWaiting = false;

        GoToRandomPoint();
    }

    private void GoToRandomPoint()
    {
        if (!isWandering)
            return;

        if (wanderPoints == null || wanderPoints.Length == 0)
            return;

        int randomIndex;

        if (wanderPoints.Length == 1)
        {
            randomIndex = 0;
        }
        else
        {
            do
            {
                randomIndex =
                    Random.Range(
                        0,
                        wanderPoints.Length
                    );
            }
            while (randomIndex == lastPointIndex);
        }

        lastPointIndex = randomIndex;

        Transform targetPoint =
            wanderPoints[randomIndex];

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