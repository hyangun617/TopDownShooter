using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(UnitAnimController), typeof(UnitHealth), typeof(UnitController))]
public abstract class Enemy : MonoBehaviour, IPoolable
{
    // 유닛 컴포넌트
    protected UnitHealth health;
    protected UnitController controller;
    protected UnitAnimController animController;

    // 자신의 풀 프리팹 참조
    public GameObject SourcePrefab { get; set; }
    private bool isSetup = false;

    public static event Action<Enemy> OnAnyEnemyDeath;      // 어느 enemy객체의 죽음 알림.

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
    public Vector3 Position => controller.Position;


    // 디버깅용 멤버
    [Header("Debug")]
    public Color bcolor = Color.green;

    // 데이터 로드를 위한 추상 메서드
    protected abstract void LoadEnemyData(int id);
    public abstract void TakeDamage(float value);

    protected virtual void Awake()
    {
        health = GetComponent<UnitHealth>();
        controller = GetComponent<UnitController>();
        animController = GetComponent<UnitAnimController>();

        // 레이어 마스크 미할당 시 폴백.
        if(targetLayerMask.value == 0)
        {
            targetLayerMask = LayerMask.GetMask("Player");
        }
    }

    public virtual void OnSpawn()
    {
        
    }

    public virtual void OnDespawn()
    {
        
    }

    public void EnsureSetup()
    {
        if(isSetup) return;
        
        RunWhenDataReady(SetupEnemy);
        isSetup = true;
    }

    // GameManager의 DataManager가 초기화 되었는지 확인하고, 초기화가 완료되었으면 SetupEnemy()를 호출. 
    // 아니면 OnDataInitialized 이벤트에 SetupEnemy()를 등록.
    protected void RunWhenDataReady(Action callback)
    {
        if(GameManager.Instance.DataMgr.IsDataInitialized)
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

    // 자식 객체는 이 메서드를 오버라이드하여 EnemyData를 로드하고, 현재 체력을 최대 체력으로 초기화 할 수 있다.
    protected virtual void SetupEnemy()
    {
        // EnemyData를 로드하고 현재 체력을 최대 체력으로 초기화
        LoadEnemyData(unitId);

        // 컴포넌트들 초기화 로직.
        health.Initialize(Stat.MaxHp);
        health.SetDamageSFX(damageSFX);
        health.SetDeathSFX(deathSFX); 

        controller.Initialize();
        animController.Initialize();
    }

    public void NotifyDeath()
    {
        OnAnyEnemyDeath?.Invoke(this);
    }

    public void ReturnToPool()
    {
        GameManager.Instance.PoolMgr.Release(gameObject);
    }

    // 위임 메서드
    // Controller
    public void MoveToward(Vector3 targetPosition, float speed) => controller.MoveToward(targetPosition, speed);
    public void StopMoving() => controller.StopMoving();
    public void Rotate(Vector3 dir) => controller.Rotate(dir);    
    
    // Animator
    public void SetMoveState(bool state) => animController.SetMoveState(state);
    public void AttackTrigger() => animController.AttackTrigger();
    public void DeathTrigger() => animController.DeathTrigger();

    // 디버깅용 범위 표시
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        // 감지 범위 표시
        // 선 색상 지정
        Handles.color = bcolor;

        // 원 그리기
        Handles.DrawWireDisc(transform.position, Vector3.up, Stat.DetectRange);  

        // 공격 범위 표시
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, Stat.AttackRange);
    }
#endif
}
