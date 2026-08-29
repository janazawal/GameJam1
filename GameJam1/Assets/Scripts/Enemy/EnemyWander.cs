using System.Collections;
using UnityEngine;

public class EnemyWander : MonoBehaviour
{
    [SerializeField]
    private Transform[] wanderPoints;

    [SerializeField]
    private float minWaitTime = 0.5f;

    [SerializeField]
    private float maxWaitTime = 1.5f;

    private EnemyMovement movement;

    private int currentPointIndex;
    private bool isWandering;

    public bool IsWandering => isWandering;

    private void Awake()
    {
        movement =
            GetComponent<EnemyMovement>();
    }

    private void OnEnable()
    {
        movement.OnDestinationReached +=
            HandleDestinationReached;
    }

    private void OnDisable()
    {
        movement.OnDestinationReached -=
            HandleDestinationReached;
    }

    public void StartWandering()
    {
        if (isWandering)
            return;

        if (wanderPoints == null ||
            wanderPoints.Length == 0)
        {
            Debug.LogWarning(
                "Enemy has no Wander Points"
            );

            return;
        }

        isWandering = true;

        GoToNextPoint();
    }

    public void StopWandering()
    {
        isWandering = false;

        StopAllCoroutines();

        movement.Stop();
    }

    private void HandleDestinationReached()
    {
        if (!isWandering)
            return;

        StartCoroutine(
            WaitThenContinue()
        );
    }

    private IEnumerator WaitThenContinue()
    {
        float delay =
            Random.Range(
                minWaitTime,
                maxWaitTime
            );

        yield return new WaitForSeconds(delay);

        if (!isWandering)
            yield break;

        GoToNextPoint();
    }

    private void GoToNextPoint()
    {
        Transform point =
            wanderPoints[currentPointIndex];

        movement.MoveTo(
            point.position
        );

        currentPointIndex++;

        if (currentPointIndex >=
            wanderPoints.Length)
        {
            currentPointIndex = 0;
        }
    }
}