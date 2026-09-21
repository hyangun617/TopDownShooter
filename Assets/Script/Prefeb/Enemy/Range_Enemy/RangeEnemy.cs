public class RangeEnemy : Enemy
{
    // 공격 컴포넌트
    private UnitRangeAttack rangeAttack;

    // AI : BehaviorTree
    private BehaviorTree behaviorTree;
    private Blackboard blackboard;

    protected override string StatTableKey => "Range_Enemy_TB";

    protected override void Awake()
    {
        base.Awake();

        rangeAttack = GetComponent<UnitRangeAttack>();
    }

    protected override void SetupEnemy()
    {
        base.SetupEnemy();

        SetupBehaviorTree();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        rangeAttack.Initialize(AttackSpeed);

        blackboard.Initialize();
        blackboard.SetValue(BlackboardKeys.Self, this);
        blackboard.SetValue(BlackboardKeys.TargetLayerMask, targetLayerMask);
        blackboard.SetValue(BlackboardKeys.ObstacleLayerMask, obstacleLayerMask);

        behaviorTree.Cancel();
        behaviorTree.Play();
    }

    protected override void HandleDeath()
    {
        behaviorTree?.Pause();
        DeathTrigger();

        // 사망 연출 후 풀 매니저에 반환
        ScheduleRelease();
    }

    private void Update()
    {
        // BT의 정지 확인 후 실행.
        if (behaviorTree != null && !behaviorTree.IsPaused)
            behaviorTree.Tick();
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

    // 트리 구조 BehaviorTree
    private INode BuildTree(Blackboard blackboard)
    {
        return new BehaviorTreeBuilder(blackboard)
            .Selector()
                .Sequence() // 공격 로직
                    .Leaf(new IsCanAttackedCondition(blackboard))
                    .Leaf(new CheckAttackDelay(blackboard, Stat.AttackDelay))
                    .Leaf(new AttackAction(blackboard))
                .End()
                .Sequence() // 감지 및 이동 로직
                    .Leaf(new IsPlayerDetectedCondition(blackboard))
                    .Leaf(new ChaseAction(blackboard))
                .End()
            .End()
            .Build();
    }
}
