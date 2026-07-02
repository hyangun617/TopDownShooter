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