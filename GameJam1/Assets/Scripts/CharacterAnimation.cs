using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    protected Animator anim;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    protected virtual void OnEnable()
    {
        PlayerKnockout knockout = GetComponent<PlayerKnockout>();
        if (knockout != null)
        {
            knockout.OnKnockedOut += HandleKnockout;
            knockout.OnRecovered += HandleRecovered;
        }
    }

    protected virtual void OnDisable()
    {
        PlayerKnockout knockout = GetComponent<PlayerKnockout>();
        if (knockout != null)
        {
            knockout.OnKnockedOut -= HandleKnockout;
            knockout.OnRecovered -= HandleRecovered;
        }
    }

    private void HandleKnockout() => SetKnockedOut(true);
    private void HandleRecovered() => SetKnockedOut(false);

    public void SetWalking(bool isWalking)
    {
        if (anim != null) anim.SetBool("isWalking", isWalking);
    }

    public void SetDashing(bool isDashing)
    {
        if (anim != null) anim.SetBool("isDashing", isDashing);
    }

    public void SetKnockedOut(bool isKnockedOut)
    {
        if (anim != null) anim.SetBool("isKnockedout", isKnockedOut);
    }
}