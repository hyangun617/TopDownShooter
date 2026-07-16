using UnityEngine;

public class UnitAnimController : MonoBehaviour
{
    private Animator animator;
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int AttackTriggerHash = Animator.StringToHash("IsAttack");
    private static readonly int DamagedHash = Animator.StringToHash("IsDamaged");
    private static readonly int DeathHash = Animator.StringToHash("IsDeath");

    public void Initialize()
    {
        animator.Rebind();
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 상태값 기반 전환
    public void SetAnimState(UnitAnimState state)
    {
        animator.SetInteger(StateHash, (int)state);
    }

    public void AttackTrigger()
    {
        animator.SetTrigger(AttackTriggerHash);
    }
    
    public void DeathTrigger()
    {
        animator.SetTrigger(DeathHash);
    }

    public void TakeDamaged(float damage)
    {
        Debug.Log("TakeDamaged!");
        animator.SetTrigger(DamagedHash);
    }
}
