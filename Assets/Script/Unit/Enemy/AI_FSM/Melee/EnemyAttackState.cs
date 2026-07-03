using UnityEngine;

public class EnemyAttackState : EnemyBaseState<MeleeEnemy>
{
    // 공격 딜레이 시간.
    private float attackTimer;

    public EnemyAttackState(MeleeEnemy enemy, EnemyStateMachine<MeleeEnemy> stateMachine) : base(enemy, stateMachine)
    {
        // 상태 초기화
    }

    public override void Enter()
    {
        // 상태 진입 시 로직
        MyGame.Utility.Debugger.Log($"{enemy.name} entered Attack State.");

        // 진입 시 공격 딜레이
        attackTimer = enemy.AttackDelay;
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

        // 공격 사거리 내면 공격 메서드 실행.
        // 공격 사거리 밖이라면 ChaseState로 상태 전환

        Vector3 targetPos = enemy.Target.position;
        Vector3 myPos = enemy.transform.position;

        // 플레이어를 향한 방향 
        Vector3 dir = (targetPos - myPos).normalized;

        // 플레이어와의 거리를 구함.
        float dist = Vector3.Distance(myPos, targetPos);

        if(dist > enemy.AttackRange)
        {
            stateMachine.ChangeState<EnemyChaseState>();   
        }

        // 사거리 안이라면 타이머가 감소해, 0 이하가 되면 공격 실행.
        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0f)
        {
            playAttack(dir);
            attackTimer = enemy.AttackDelay;
        }
    }

    public override void Exit()
    {
        // 상태 종료 로직
    }

    // 공격 실행 메서드.
    private void playAttack(Vector3 dir)
    {
        MyGame.Utility.Debugger.Log($"{enemy.name}'s Attack!");

        // 실제 데미지 판정.

    }
}