public class MeleeEnemy : Enemy
{
    // AI : FSM
    private EnemyStateMachine<MeleeEnemy> stateMachine;
    private EnemyFSM_Context context;

    protected override string StatTableKey => "Melee_Enemy_TB";

    protected override void SetupEnemy()
    {
        base.SetupEnemy();

        // 상태 머신 초기화
        stateMachine = new EnemyStateMachine<MeleeEnemy>();
        context = new EnemyFSM_Context();
        stateMachine.Register(new EnemyIdleState(this, stateMachine, context));
        stateMachine.Register(new EnemyChaseState(this, stateMachine, context));
        stateMachine.Register(new EnemyAttackState(this, stateMachine, context));
        stateMachine.Register(new EnemyDeadState(this, stateMachine, context));

        stateMachine.Initialize<EnemyIdleState>();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        context.Initialize();
        stateMachine.ChangeState<EnemyIdleState>();
    }

    protected override void HandleDeath()
    {
        // FSM 상태를 죽음 상태로 변화.
        stateMachine.ChangeState<EnemyDeadState>();
    }

    // 데미지 판정 (애니메이션 이벤트 "Attack")
    public void Attack()
    {
        attack.PlayAttack();
    }

    private void Update()
    {
        // 상태 머신 업데이트 -> FSM AI
        stateMachine?.Update();
    }
}
