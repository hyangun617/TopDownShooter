using UnityEngine;

public class RangeEnemy : Enemy
{
    // 공격 컴포넌트
    public UnitRangeAttack rangeAttack; 

    // AI : BehaviorTree
    private BehaviorTree behaviorTree;
    private Blackboard blackboard;

    protected override void Awake()
    {
        base.Awake();

        rangeAttack = GetComponent<UnitRangeAttack>();
        targetLayerMask = LayerMask.GetMask("Player");
        obstacleLayerMask = LayerMask.GetMask("Environment");
    }

    protected override void SetupEnemy()
    {
        base.SetupEnemy();

        SetupBehaviorTree();
    }

    public override void OnSpawn()
    {
        health.Initialize(Stat.MaxHp);
        controller.Initialize();
        rangeAttack.Initialize();
        animController.Initialize();
        blackboard.Initialize();

        blackboard.SetValue(BlackboardKeys.Self, this);
        blackboard.SetValue(BlackboardKeys.TargetLayerMask, targetLayerMask);
        blackboard.SetValue(BlackboardKeys.ObstacleLayerMask, obstacleLayerMask);

        health.OnDeath += HandleDeath;
        health.OnDamaged += animController.TakeDamaged;

        behaviorTree.Cancel();
        behaviorTree.Play();

        base.OnSpawn();
    }

    public override void OnDespawn()
    {
        base.OnDespawn(); 

        health.OnDeath -= HandleDeath;
        health.OnDamaged -= animController.TakeDamaged;
    }

    private void HandleDeath()
    {
        Debug.Log($"{name} is Dead!");
        behaviorTree?.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (behaviorTree != null && !behaviorTree.IsPaused)
        {
            // BT의 정지 확인 후 실행.
            behaviorTree.Tick();   
        }
    }

    // 원거리 데이터 테이블에서 Id 값으로 스탯 값 로드
    protected override void LoadEnemyData(int id)
    {
        Stat = GameManager.Instance.DataMgr.rangeEnemyTB.GetEnemyDataById(id);

        // Table Data 로드 실패.
        if(Stat == null)
        {
            Debug.LogError($"[EnemyTB] id : {id}에 해당하는 EnemyData가 Range_Enemy_TB에 존재하지 않습니다.");
        }

        rangeAttack.AttackDamage = Stat.AttackPoint;
        rangeAttack.AttackDelay = Stat.AttackDelay;
        rangeAttack.AttackRange = Stat.AttackRange;
        rangeAttack.targetLayerMask = targetLayerMask;
    }

    private void SetupBehaviorTree()
    {
        // 블랙보드의 값 설정
        blackboard = new Blackboard();
        blackboard.SetValue(BlackboardKeys.Self, this);
        blackboard.SetValue(BlackboardKeys.TargetLayerMask, targetLayerMask);
        blackboard.SetValue(BlackboardKeys.ObstacleLayerMask, obstacleLayerMask);

        // BehaviorTree 생성 및 루트 노드 설정
        INode root = BuildTree(blackboard);
        behaviorTree = new BehaviorTree(root, blackboard);
        behaviorTree.SetDelay(0.1f);
    }

    public void PlayAttack() => rangeAttack.PlayAttack();

    // 트리 구조 BehaviorTree
    private INode BuildTree(Blackboard blackboard)
    {
        // 트리 구조 구성
        return new BehaviorTreeBuilder(blackboard)
            .Selector()
                .Sequence() // 공격 로직
                    .Leaf(new IsCanAttackedCondition(blackboard))
                    .Leaf(new AttackAction(blackboard, Stat.AttackDelay))
                .End()
                .Sequence() // 감지 및 이동 로직
                    .Leaf(new IsPlayerDetectedCondition(blackboard))
                    .Leaf(new ChaseAction(blackboard))
                .End()
            .End()
            .Build();
    }
}
