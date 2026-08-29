using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    private EnemyDetection detection;
    private EnemyShopping shopping;
    private EnemyWander wander;
    private EnemyCombat combat;

    private void Awake()
    {
        detection =
            GetComponentInChildren<EnemyDetection>();

        shopping =
            GetComponent<EnemyShopping>();

        wander =
            GetComponent<EnemyWander>();

        combat =
            GetComponent<EnemyCombat>();
    }

    private void OnEnable()
    {
        if (detection != null)
        {
            detection.OnPlayerDetected +=
                HandlePlayerDetected;

            detection.OnPlayerLost +=
                HandlePlayerLost;
        }

        if (shopping != null)
        {
            shopping.OnShoppingCompleted +=
                HandleShoppingCompleted;
        }
    }

    private void OnDisable()
    {
        if (detection != null)
        {
            detection.OnPlayerDetected -=
                HandlePlayerDetected;

            detection.OnPlayerLost -=
                HandlePlayerLost;
        }

        if (shopping != null)
        {
            shopping.OnShoppingCompleted -=
                HandleShoppingCompleted;
        }
    }

    private void HandlePlayerDetected(
        Transform player)
    {
        shopping.PauseShopping();

        wander.StopWandering();

        combat.StartCombat(player);
    }

    private void HandlePlayerLost(
        Transform player)
    {
        combat.StopCombat();

        if (shopping.IsShoppingComplete)
        {
            wander.StartWandering();
        }
        else
        {
            shopping.ResumeShopping();
        }
    }

    private void HandleShoppingCompleted()
    {
        if (!combat.IsInCombat)
        {
            wander.StartWandering();
        }
    }
}