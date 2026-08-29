using UnityEngine;

public class CartAttackHitbox : MonoBehaviour
{
    private bool canHit;

    public void EnableHitbox()
    {
        canHit = true;
    }

    public void DisableHitbox()
    {
        canHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canHit)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerKnockout knockout =
            other.GetComponent<PlayerKnockout>();

        if (knockout == null)
            return;

        knockout.ApplyKnockout();

        canHit = false;
    }
}