using UnityEngine;

public class MeleeEnemy : Enemy
{
    private EnemyStateMachine<MeleeEnemy> stateMachine;

    // State에서 Stat에 접근하기 위한 공개 프로퍼티
    public float MoveSpeed => Stat.MovementSpeed;
    public float AttackRange => Stat.AttackRange;
    public float DetectRange => Stat.DetectRange;
    public float CurrentHp => currentHp;

    // 타겟에 접근하기 위한 프로퍼티
    public Transform Target => target;
    public LayerMask TargetLayerMask => targetLayerMask;
    public LayerMask ObstacleLayerMask => obstacleLayerMask;

    protected override void SetupEnemy()
    {
        base.SetupEnemy();

        // 상태 머신 초기화
        stateMachine = new EnemyStateMachine<MeleeEnemy>();
        stateMachine.Register(new EnemyIdleState(this, stateMachine));
        stateMachine.Register(new EnemyChaseState(this, stateMachine));
        stateMachine.Register(new EnemyAttackState(this, stateMachine));
        stateMachine.Register(new EnemyDeadState(this, stateMachine));

        stateMachine.Initialize<EnemyIdleState>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        // 상태 머신 업데이트 -> FSM AI
        stateMachine?.Update();
    }

    public override void TakeDamage(float value)
    {
        base.TakeDamage(value); // 데미지 계산 + 이벤트 호출 로직

        // 체력 체크 후 상태 전환
        if (currentHp <= 0)
        {
            stateMachine.ChangeState<EnemyDeadState>();
        }
    }
}