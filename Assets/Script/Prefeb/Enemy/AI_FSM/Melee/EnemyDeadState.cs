public class EnemyDeadState : EnemyBaseState<MeleeEnemy>
{
    public EnemyDeadState(MeleeEnemy enemy, EnemyStateMachine<MeleeEnemy> stateMachine, EnemyFSM_Context context) : base(enemy, stateMachine, context)
    {
        // 상태 초기화
    }

    public override void Enter()
    {
        enemy.StopMoving();
        enemy.DeathTrigger();

        // 사망 연출 후 풀에 반환.
        enemy.ScheduleRelease();
    }

    public override void Update()
    {
        // 상태 업데이트 로직
    }

    public override void Exit()
    {
        // 상태 종료 로직
    }
}