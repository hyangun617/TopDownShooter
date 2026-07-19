using UnityEngine;

public class UnitAnimController : MonoBehaviour
{
    private Animator animator;
    private static readonly int MoveHash = Animator.StringToHash("OnMove");
    private static readonly int AttackTriggerHash = Animator.StringToHash("IsAttack");
    private static readonly int DamagedHash = Animator.StringToHash("IsDamaged");
    private static readonly int DeathHash = Animator.StringToHash("IsDeath");

    public void Initialize()
    {
        animator.Rebind();
        animator.Update(0f);
        animator.ResetControllerState();
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AttackTrigger()
    {
        animator.SetTrigger(AttackTriggerHash);
    }

    public void SetMoveState(bool onMove)
    {
        animator.SetBool(MoveHash, onMove);
    }
    
    public void DeathTrigger()
    {
        animator.SetTrigger(DeathHash);
    }

    public void TakeDamaged()
    {
        animator.SetTrigger(DamagedHash);
    }
}
