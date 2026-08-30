using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    private EnemyDetection detection;
    private EnemyShopping shopping;
    private EnemyWander wander;
    private EnemyCombat combat;
    private EnemyKnockout enemyKnockout;

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

        enemyKnockout =
            GetComponent<EnemyKnockout>();
    }

    private void OnEnable()
    {
        if (detection != null)
        {
            detection.OnPlayerDetected += HandlePlayerDetected;
            detection.OnPlayerLost += HandlePlayerLost;
        }

        if (shopping != null)
        {
            shopping.OnShoppingCompleted += HandleShoppingCompleted;
        }
    }

    private void OnDisable()
    {
        if (detection != null)
        {
            detection.OnPlayerDetected -= HandlePlayerDetected;
            detection.OnPlayerLost -= HandlePlayerLost;
        }

        if (shopping != null)
        {
            shopping.OnShoppingCompleted -= HandleShoppingCompleted;
        }
    }

    private void HandlePlayerDetected(Transform player)
    {
        // Enemy نفسه Knocked Out
        if (enemyKnockout != null &&
            enemyKnockout.IsKnockedOut)
        {
            return;
        }

        if (player == null)
            return;

        PlayerKnockout playerKnockout =
            player.GetComponent<PlayerKnockout>();

        // متهاجمش Player واقع
        if (playerKnockout != null &&
            playerKnockout.IsKnockedOut)
        {
            return;
        }

        if (shopping != null)
        {
            shopping.PauseShopping();
        }

        if (wander != null)
        {
            wander.StopWandering();
        }

        if (combat != null)
        {
            combat.StartCombat(player);
        }

        Debug.Log("Enemy -> COMBAT");
    }

    private void HandlePlayerLost(Transform player)
    {
        LeaveCombatAndResumeTask();
    }

    public void LeaveCombatAndResumeTask()
    {
        if (combat != null)
        {
            combat.StopCombat();
        }

        if (shopping != null &&
            !shopping.IsShoppingComplete)
        {
            shopping.ResumeShopping();

            Debug.Log("Enemy -> SHOPPING");
        }
        else if (wander != null)
        {
            wander.StartWandering();

            Debug.Log("Enemy -> WANDERING");
        }
    }

    private void HandleShoppingCompleted()
    {
        if (combat != null &&
            !combat.IsInCombat)
        {
            if (wander != null)
            {
                wander.StartWandering();
            }
        }
    }
}