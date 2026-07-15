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

        health.OnDeath += HandleDeath;
        health.OnDamaged += animController.TakeDamaged;
    }

    protected override void LoadEnemyData(int id)
    {
        Stat = GameManager.Instance.Data.meleeEnemyTB.GetEnemyDataById(id);

        // Table Data 로드 실패.
        if(Stat == null)
        {
            Debug.LogError($"[EnemyTB] id : {id}에 해당하는 EnemyData가 Melee_Enemy_TB에 존재하지 않습니다.");
        }

        meleeAttack.AttackDamage = Stat.AttackPoint;
        meleeAttack.AttackRange = Stat.AttackRange;
        meleeAttack.AttackDelay = Stat.AttackDelay;

        // 미리 설정된 TB의 오디오 클립 값이 있는 경우.
        if(Stat.AttackSFX != null)
            meleeAttack.SetAttackSFX(Stat.AttackSFX);
        else
            meleeAttack.SetAttackSFX(attackSFX);
    }

    // 죽을 시 호출되는 메서드.
    private void HandleDeath()
    {
        // FSM 상태를 죽음 상태로 변화.
        stateMachine.ChangeState<EnemyDeadState>();
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