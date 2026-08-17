using UnityEngine;
using System.Collections;

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
        rangeAttack.SetAttackSFX(attackSFX);
    }

    public override void OnSpawn()
    {
        health.Initialize(Stat.MaxHp);
        controller.Initialize();
        rangeAttack.Initialize(AttackSpeed);
        animController.Initialize();
        blackboard.Initialize();

        blackboard.SetValue(BlackboardKeys.Self, this);
        blackboard.SetValue(BlackboardKeys.TargetLayerMask, targetLayerMask);
        blackboard.SetValue(BlackboardKeys.ObstacleLayerMask, obstacleLayerMask);

        health.OnDeath += HandleDeath;
        health.OnDamaged += TakeDamage;

        behaviorTree.Cancel();
        behaviorTree.Play();

        base.OnSpawn();
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

    private void HandleDeath()
    {
        behaviorTree?.Pause();
        animController.DeathTrigger();        
        NotifyDeath();

        // 할당 점수 +
        GameManager.Instance.SetScore(GameManager.Instance.Score + Stat.Score);

        // 풀 매니저에 반환
        StartCoroutine(ReleaseAfterDelay());
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
        Stat = GameManager.Instance.DataMgr.Get<EnemyTB>("Range_Enemy_TB").GetEnemyDataById(id);

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

    // 트리 구조 BehaviorTree
    private INode BuildTree(Blackboard blackboard)
    {
        // 트리 구조 구성
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

    private IEnumerator ReleaseAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        ReturnToPool();
    }
}
