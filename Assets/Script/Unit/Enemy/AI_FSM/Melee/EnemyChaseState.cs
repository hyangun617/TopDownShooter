using UnityEngine;

public class EnemyChaseState : EnemyBaseState<MeleeEnemy>
{
    public EnemyChaseState(MeleeEnemy enemy, EnemyStateMachine<MeleeEnemy> stateMachine) : base(enemy, stateMachine)
    {
        // 상태 초기화
    }

    public override void Enter()
    {
        // 상태 진입 시 로직
        MyGame.Utility.Debugger.Log($"{enemy.name} entered Chase State.");

        enemy.bcolor = Color.red;
    }

    public override void Update()
    {
        // 상태 업데이트 로직
        // target이 존재하지 않는 경우 Idle 상태로 변화
        if(enemy.Target == null)
        {
            stateMachine.ChangeState<EnemyIdleState>();
            return;
        }

        // 플레이어의 위치를 받아와 거리를 계산
        Vector3 targetPos = enemy.Target.position;
        Vector3 myPos = enemy.transform.position;

        // 방향 벡터를 구해 일반화.
        Vector3 dir = (targetPos - myPos).normalized;
        
        // 해당 방향으로 이동.
        enemy.transform.position += dir * enemy.MoveSpeed * Time.deltaTime;

        // 거리가 탐지 범위를 벗어나면 -> idle
        // 거리가 공격 범위 내라면 -> attack
        // 거리가 탐지 범위 내, 공격 범위 밖이라면 유지.
        float dist = Vector3.Distance(myPos, targetPos);
        if(dist <= enemy.AttackRange)
        {
            stateMachine.ChangeState<EnemyAttackState>();
        }
        else if(dist > enemy.DetectRange)
        {
            stateMachine.ChangeState<EnemyIdleState>();
        }        
    }

    public override void Exit()
    {
        // 상태 종료 로직
    }
}