using UnityEngine;
using System.Collections;

public class MeleeEnemy : Enemy
{
    private EnemyStateMachine<MeleeEnemy> stateMachine;
    private EnemyFSM_Context context;

    // 공격 컴포넌트
    public UnitMeleeAtack meleeAttack;

    protected override void Awake()
    {
        base.Awake();
        // 공격 컴포넌트 
        meleeAttack = GetComponent<UnitMeleeAtack>();
    }

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

        health.OnDeath += HandleDeath;
    }

    protected override void LoadEnemyData(int id)
    {
        Stat = GameManager.Instance.Data.meleeEnemyTB.GetEnemyDataById(id);

        meleeAttack.AttackDamage = Stat.AttackPoint;
        meleeAttack.AttackRange = Stat.AttackRange;
        meleeAttack.AttackDelay = Stat.AttackDelay;
    }

    private void HandleDeath()
    {
        stateMachine.ChangeState<EnemyDeadState>();
    }

    public void PlayAttack() => meleeAttack.PlayAttack();

    void Update()
    {
        // 상태 머신 업데이트 -> FSM AI
        stateMachine?.Update();
    }
}