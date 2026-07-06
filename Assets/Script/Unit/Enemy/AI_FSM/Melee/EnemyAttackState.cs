using UnityEngine;

public class EnemyAttackState : EnemyBaseState<MeleeEnemy>
{
    // 공격 딜레이 시간.
    private float attackTimer;

    public EnemyAttackState(MeleeEnemy enemy, EnemyStateMachine<MeleeEnemy> stateMachine, EnemyFSM_Context context) : base(enemy, stateMachine, context)
    {
        // 상태 초기화
    }

    public override void Enter()
    {
        // 상태 진입 시 로직
        MyGame.Utility.Debugger.Log($"{enemy.name} entered Attack State.");

        // 진입 시 공격 딜레이
        attackTimer = 0f;
    }

    public override void Update()
    {
        // 상태 업데이트 로직
        // target이 존재하지 않는 경우 Idle 상태로 변화
        if(context.Target == null)
        {
            stateMachine.ChangeState<EnemyIdleState>();
            return;
        }

        enemy.StopMoving();

        // 딜레이(후딜) 진행 중 
        // 사거리 판단 보류 및 제자리 대기
        attackTimer -= Time.deltaTime;
        if(attackTimer > 0f)
        {
            return;
        }

        // 딜레이가 끝난 후 공격 및 추격을 재판단함.
        // 공격 사거리 내면 공격 메서드 실행.
        // 공격 사거리 밖이라면 ChaseState로 상태 전환
        // y축 벡터 값은 무시함.
        Vector3 targetPos = context.Target.position;
        Vector3 myPos = enemy.transform.position;
        Vector3 dir = targetPos - myPos;
        dir.y = 0f;
        dir.Normalize();

        enemy.transform.rotation = Quaternion.LookRotation(dir);

        // 플레이어와의 거리를 구함.
        float dist = Vector3.Distance(myPos, targetPos);

        if(dist > enemy.AttackRange)
        {
            stateMachine.ChangeState<EnemyChaseState>();   
            return;
        }

        enemy.PlayAttack();
        attackTimer = enemy.AttackDelay;
    }

    public override void Exit()
    {
        // 상태 종료 로직
    }
}