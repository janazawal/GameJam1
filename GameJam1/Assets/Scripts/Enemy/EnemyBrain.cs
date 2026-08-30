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

    private void HandlePlayerDetected(Transform player)
    {
        shopping.PauseShopping();

        wander.StopWandering();

        combat.StartCombat(player);
    }

    private void HandlePlayerLost(Transform player)
    {
        LeaveCombatAndResumeTask();
    }

    public void LeaveCombatAndResumeTask()
    {
        combat.StopCombat();

        if (shopping != null &&
            !shopping.IsShoppingComplete)
        {
            shopping.ResumeShopping();

            Debug.Log("Enemy returned to Shopping");
        }
        else
        {
            wander.StartWandering();

            Debug.Log("Enemy returned to Wandering");
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