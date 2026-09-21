using UnityEngine;

// 유닛 공통 애니메이션 제어. 파라미터 이름이 다른 애니메이터(예: Player)는 MoveParam / AttackParam 을 override 한다.
public class UnitAnimController : MonoBehaviour
{
    protected Animator animator;

    private static readonly int MoveHash = Animator.StringToHash("OnMove");
    private static readonly int AttackTriggerHash = Animator.StringToHash("IsAttack");
    private static readonly int DamagedHash = Animator.StringToHash("IsDamaged");
    private static readonly int DeathHash = Animator.StringToHash("IsDeath");

    protected virtual int MoveParam => MoveHash;
    protected virtual int AttackParam => AttackTriggerHash;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize()
    {
        animator.Rebind();
        animator.Update(0f);
        animator.ResetControllerState();
    }

    public void AttackTrigger() => animator.SetTrigger(AttackParam);

    public void SetMoveState(bool onMove) => animator.SetBool(MoveParam, onMove);

    public void DeathTrigger() => animator.SetTrigger(DeathHash);

    public void TakeDamaged() => animator.SetTrigger(DamagedHash);
}
