using UnityEngine;

public class MeleeEnemy : Enemy
{
    private EnemyStateMachine<MeleeEnemy> stateMachine;
    private EnemyFSM_Context context;

    // 공격 컴포넌트
    private UnitMeleeAtack meleeAttack;

    protected override void Awake()
    {
        base.Awake();
        // 공격 컴포넌트 
        meleeAttack = GetComponent<UnitMeleeAtack>();
        targetLayerMask = LayerMask.GetMask("Player");
        obstacleLayerMask = LayerMask.GetMask("Environment");
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

        meleeAttack.SetAttackSFX(attackSFX);
    }

    public override void OnSpawn()
    {
        health.Initialize(Stat.MaxHp);
        context.Initialize();
        controller.Initialize();
        animController.Initialize();

        health.OnDeath += HandleDeath;
        health.OnDamaged += TakeDamage;

        base.OnSpawn();

        stateMachine.ChangeState<EnemyIdleState>();
    }

    public override void OnDespawn()
    {
        base.OnDespawn(); 

        health.OnDeath -= HandleDeath;
        health.OnDamaged -= TakeDamage;
    }

    public override void TakeDamage(float vaule)
    {
        animController.TakeDamaged();
    }

    protected override void LoadEnemyData(int id)
    {
        Stat = GameManager.Instance.DataMgr.Get<EnemyTB>("GameData_Melee_Enemy_TB").GetEnemyDataById(id);

        meleeAttack.AttackDamage = Stat.AttackPoint;
        meleeAttack.AttackRange = Stat.AttackRange;
        meleeAttack.AttackDelay = Stat.AttackDelay;
    }

    // 죽을 시 호출되는 메서드.
    private void HandleDeath()
    {
        // FSM 상태를 죽음 상태로 변화.
        stateMachine.ChangeState<EnemyDeadState>();

        // 할당 점수 +
        GameManager.Instance.SetScore(GameManager.Instance.Score + Stat.Score);
        
        NotifyDeath();
    }

    // 데미지 판정 (애니메이션 이벤트)
    public void Attack()
    {
        // 공격 범위의 객체 판정, 데미지
        meleeAttack.PlayAttack();
    }

    void Update()
    {
        // 상태 머신 업데이트 -> FSM AI
        stateMachine?.Update();
    }
}