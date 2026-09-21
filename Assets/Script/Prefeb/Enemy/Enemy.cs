using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Enemy : Unit, IPoolable
{
    // 자신의 풀 프리팹 참조
    public GameObject SourcePrefab { get; set; }
    private bool isSetup = false;

    public static event Action<Enemy> OnAnyEnemyDeath;      // 어느 enemy객체의 죽음 알림.

    // 풀에 반환되기 전 사망 연출을 보여주는 시간.
    private const float ReleaseDelay = 5f;

    // 데이터 값을 지정할 Id;
    [Header("Read Stat Data Table Id")]
    [SerializeField] protected int unitId;

    // 기본 스텟 데이터
    [Header("Enemy Data")]
    [SerializeField] protected EnemyData Stat;

    // 판정을 위한 레이어 마스크
    [Header("Target Layer Mask")]
    [SerializeField] protected LayerMask targetLayerMask;
    [SerializeField] protected LayerMask obstacleLayerMask;

    [Header("SFX")]
    [SerializeField] protected List<AudioClip> attackSFX;
    [SerializeField] protected List<AudioClip> damageSFX;
    [SerializeField] protected List<AudioClip> deathSFX;

    // 외부 접근을 위한 프로퍼티
    // BT, FSM 등에서 사용함.
    public LayerMask TargetLayerMask => targetLayerMask;
    public LayerMask ObstacleLayerMask => obstacleLayerMask;
    public float AttackRange => Stat.AttackRange;
    public float AttackSpeed => Stat.AttackSpeed;
    public float AttackPoint => Stat.AttackPoint;
    public float AttackDelay => Stat.AttackDelay;
    public float DetectRange => Stat.DetectRange;
    public float MoveSpeed => Stat.MovementSpeed;

    // 디버깅용 멤버
    [Header("Debug")]
    public Color bcolor = Color.green;

    // 스탯을 읽어올 데이터 테이블의 키.
    protected abstract string StatTableKey { get; }

    // 사망 시 종류별 연출 (FSM 상태 전환, BT 정지 등). 점수 / 사망 알림은 Enemy가 공통 처리한다.
    protected abstract void HandleDeath();

    protected override void Awake()
    {
        base.Awake();

        // 레이어 마스크 미할당 시 폴백.
        if (targetLayerMask.value == 0)
            targetLayerMask = LayerMask.GetMask("Player");

        if (obstacleLayerMask.value == 0)
            obstacleLayerMask = LayerMask.GetMask("Environment");
    }

    // 풀에서 꺼내질 때 호출. 자식 클래스는 base.OnSpawn() 이후 자신의 초기화를 이어서 한다.
    public virtual void OnSpawn()
    {
        health.Initialize(Stat.MaxHp);
        controller.Initialize();
        animController.Initialize();

        health.OnDeath += OnHealthDeath;
        health.OnDamaged += HandleDamaged;
    }

    public virtual void OnDespawn()
    {
        health.OnDeath -= OnHealthDeath;
        health.OnDamaged -= HandleDamaged;
    }

    public void EnsureSetup()
    {
        if (isSetup) return;

        RunWhenDataReady(SetupEnemy);
        isSetup = true;
    }

    // GameManager의 DataManager가 초기화 되었는지 확인하고, 초기화가 완료되었으면 callback을 호출.
    // 아니면 OnDataInitialized 이벤트에 callback을 등록.
    protected void RunWhenDataReady(Action callback)
    {
        if (GameManager.Instance.DataMgr.IsDataInitialized)
            callback();
        else
        {
            // 콜백 실행 후 자동으로 구독 해제되도록 래핑
            void Handler()
            {
                GameManager.Instance.DataMgr.OnDataInitialized -= Handler;
                callback();
            }
            GameManager.Instance.DataMgr.OnDataInitialized += Handler;
        }
    }

    // 데이터 테이블에서 스탯을 읽어 각 컴포넌트에 적용한다. 자식 객체는 오버라이드하여 추가 초기화를 할 수 있다.
    protected virtual void SetupEnemy()
    {
        Stat = GameManager.Instance.DataMgr.Get<EnemyTB>(StatTableKey).GetEnemyDataById(unitId);

        health.SetDamageSFX(damageSFX);
        health.SetDeathSFX(deathSFX);

        attack.AttackDamage = Stat.AttackPoint;
        attack.AttackRange = Stat.AttackRange;
        attack.AttackDelay = Stat.AttackDelay;
        attack.TargetLayerMask = targetLayerMask;
        attack.SetAttackSFX(attackSFX);
    }

    private void OnHealthDeath()
    {
        HandleDeath();

        // 할당 점수 +
        GameManager.Instance.SetScore(GameManager.Instance.Score + Stat.Score);

        OnAnyEnemyDeath?.Invoke(this);
    }

    // 피격 시 반응.
    protected virtual void HandleDamaged(float currentHp)
    {
        animController.TakeDamaged();
    }

    public void ReturnToPool()
    {
        GameManager.Instance.PoolMgr.Release(gameObject);
    }

    // 일정 시간 뒤 풀에 반환. (비활성화되면 코루틴이 함께 종료되므로 별도 취소는 필요 없음)
    public void ScheduleRelease()
    {
        StartCoroutine(ReleaseAfterDelay());
    }

    private IEnumerator ReleaseAfterDelay()
    {
        yield return new WaitForSeconds(ReleaseDelay);
        ReturnToPool();
    }

    // 디버깅용 범위 표시
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        // 감지 범위 표시
        Handles.color = bcolor;
        Handles.DrawWireDisc(transform.position, Vector3.up, Stat.DetectRange);

        // 공격 범위 표시
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, Stat.AttackRange);
    }
#endif
}
