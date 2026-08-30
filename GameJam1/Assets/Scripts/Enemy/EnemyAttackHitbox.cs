using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private bool canHit = false;

    public void EnableHit()
    {
        canHit = true;

        Debug.Log("ATTACK HITBOX ENABLED");
    }

    public void DisableHit()
    {
        canHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHitPlayer(other);
    }

    // مهم جدًا:
    // لو Player كان بالفعل جوه الـTrigger
    private void OnTriggerStay(Collider other)
    {
        TryHitPlayer(other);
    }

    private void TryHitPlayer(Collider other)
    {
        if (!canHit)
            return;

        PlayerKnockout playerKnockout =
            other.GetComponentInParent<PlayerKnockout>();

        if (playerKnockout == null)
            return;

        if (playerKnockout.IsKnockedOut)
            return;

        Debug.Log("CART HIT PLAYER -> KNOCKOUT");

        playerKnockout.ApplyKnockout();

        // يمنع إنه يضربه كل frame
        canHit = false;
    }
}