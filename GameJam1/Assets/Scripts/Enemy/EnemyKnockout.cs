using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyKnockout : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private KnockoutTimerUI knockoutTimerUI;

    private bool isKnockedOut;

    private NavMeshAgent agent;
    private EnemyMovement movement;
    private EnemyCombat combat;
    private EnemyShopping shopping;
    private EnemyWander wander;
    private EnemyBrain brain;

    public bool IsKnockedOut => isKnockedOut;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
        shopping = GetComponent<EnemyShopping>();
        wander = GetComponent<EnemyWander>();
        brain = GetComponent<EnemyBrain>();
    }

    public void TakeKnockout(Vector3 hitDirection)
    {
        if (isKnockedOut)
            return;

        StartCoroutine(KnockoutRoutine());
    }

    private IEnumerator KnockoutRoutine()
    {
        isKnockedOut = true;

        // 1. تشغيل أنيميشن الـ Knockout والنجوم للعدو
        CharacterAnimation anim = GetComponent<CharacterAnimation>();
        if (anim != null)
        {
            anim.SetKnockedOut(true);
        }

        // 2. إيقاف الحركة والـ NavMeshAgent تماماً
        if (movement != null)
        {
            movement.Stop();
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false; // إيقاف الناف ميكس نهائياً
        }

        // 3. تعطيل كل سكربتات الذكاء الاصطناعي للعدو نهائياً
        if (combat != null) combat.enabled = false;
        if (shopping != null) shopping.enabled = false;
        if (wander != null) wander.enabled = false;
        if (brain != null) brain.enabled = false;

        // 4. إخفاء واجهة الـ UI الخاصة بالتعديل لو موجودة
        if (knockoutTimerUI != null)
        {
            knockoutTimerUI.Hide();
        }

        // مسحنا كود الـ Recover أوتوماتيك عشان يفضل مغمى عليه طول الجيم
        yield break;
    }
}