using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Enemy : MonoBehaviour
{
    // 유닛 컴포넌트
    protected UnitHealth health;
    protected UnitController controller;

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

    // 외부 접근을 위한 프로퍼티
    // BT, FSM 등에서 사용함.
    public LayerMask TargetLayerMask => targetLayerMask;
    public LayerMask ObstacleLayerMask => obstacleLayerMask;
    public float AttackRange => Stat.AttackRange;
    public float AttackPoint => Stat.AttackPoint;
    public float AttackDelay => Stat.AttackDelay;
    public float DetectRange => Stat.DetectRange;
    public float MoveSpeed => Stat.MovementSpeed;

    // 디버깅용 멤버
    public Color bcolor = Color.green;

    // 데이터 로드를 위한 추상 메서드
    protected abstract void LoadEnemyData(int id);

    protected virtual void Awake()
    {
        health = GetComponent<UnitHealth>();
        controller = GetComponent<UnitController>();

        // 레이어 마스크 미할당 시 폴백.
        if(targetLayerMask.value == 0)
        {
            targetLayerMask = LayerMask.GetMask("Player");
        }
    }

    protected virtual void Start()
    {
        RunWhenDataReady(SetupEnemy);
    }

    // GameManager의 DataManager가 초기화 되었는지 확인하고, 초기화가 완료되었으면 SetupEnemy()를 호출. 
    // 아니면 OnDataInitialized 이벤트에 SetupEnemy()를 등록.
    protected void RunWhenDataReady(Action callback)
    {
        if(GameManager.Instance.Data.IsDataInitialized)
            callback();
        else
        {
            // 콜백 실행 후 자동으로 구독 해제되도록 래핑
            void Handler()
            {
                GameManager.Instance.Data.OnDataInitialized -= Handler;
                callback();
            }   
            GameManager.Instance.Data.OnDataInitialized += Handler;
        }            
    }

    // 자식 객체는 이 메서드를 오버라이드하여 EnemyData를 로드하고, 현재 체력을 최대 체력으로 초기화 할 수 있다.
    protected virtual void SetupEnemy()
    {
        // EnemyData를 로드하고 현재 체력을 최대 체력으로 초기화
        LoadEnemyData(unitId);

        // 컴포넌트들 초기화 로직.
        health.Initialize(Stat.MaxHp);        
    }

    // Enemy AI를 위한 위임 메서드
    public void MoveToward(Vector3 targetPosition, float speed) => controller.MoveToward(targetPosition, speed);
    public void StopMoving() => controller.StopMoving();

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
