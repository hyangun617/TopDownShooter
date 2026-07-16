using System.Collections;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState<MeleeEnemy>
{
    private Coroutine releaseRoutine;

    public EnemyDeadState(MeleeEnemy enemy, EnemyStateMachine<MeleeEnemy> stateMachine, EnemyFSM_Context context) : base(enemy, stateMachine, context)
    {
        // 상태 초기화
    }

    public override void Enter()
    {
        // 상태 진입 시 로직
        MyGame.Utility.Debugger.Log($"{enemy.name} entered Dead State.");

        enemy.StopMoving();
        enemy.DeathTrigger();

        // 5초후 비활성화.
        releaseRoutine = enemy.StartCoroutine(ReleaseAfterDelay());
    }

    private IEnumerator ReleaseAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        
        // 매니저 풀에 반환.
        enemy.ReturnToPool();
    }

    public override void Update()
    {
        // 상태 업데이트 로직
    }

    public override void Exit()
    {
        // 상태 종료 로직
        // 만약 코루틴이 실행 중이라면 종료.
        if(releaseRoutine != null)
            enemy.StopCoroutine(releaseRoutine);
    }
}