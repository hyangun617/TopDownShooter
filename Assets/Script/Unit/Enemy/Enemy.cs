using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Enemy : UnitBase, IDamagable
{
    protected Rigidbody rb;

    // 데이터 값을 지정할 Id;
    [Header("Read Stat Data Table Id")]
    [SerializeField] protected int unitId;

    // 기본 스텟 데이터
    [Header("Enemy Data")]
    [SerializeField] protected EnemyData Stat;

    // 런타임 멤버 값.
    [Header("Runtime Data")]
    [SerializeField] protected float currentHp;

    
    // State에서 Stat에 접근하기 위한 공개 프로퍼티
    public float MoveSpeed => Stat.MovementSpeed;
    public float AttackRange => Stat.AttackRange;
    public float AttackDelay => Stat.AttackDelay;
    public float DetectRange => Stat.DetectRange;
    public float CurrentHp => currentHp;

    // 판정을 위한 레이어 마스크
    [Header("Target Layer Mask")]
    [SerializeField] protected LayerMask targetLayerMask;
    [SerializeField] protected LayerMask obstacleLayerMask;
    [SerializeField] protected Transform target;

    // 타겟에 접근하기 위한 프로퍼티
    public Transform Target => target;
    public LayerMask TargetLayerMask => targetLayerMask;
    public LayerMask ObstacleLayerMask => obstacleLayerMask;

    // 이동 의도 기록 멤버
    protected Vector3 pendingMoveDirection = Vector3.zero;
    protected float pendingMoveSpeed = 0f;

    // 피격시 호출 할 이벤트.
    public Action<float> takeDamageEvent;

    // 디버깅용 멤버
    public Color bcolor = Color.green;
    

    // 데이터 로드를 위한 추상 메서드
    protected abstract void LoadEnemyData(int id);

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        // GameManager의 DataManager가 초기화 되었는지 확인하고, 초기화가 완료되었으면 SetupEnemy()를 호출. 
        // 아니면 OnDataInitialized 이벤트에 SetupEnemy()를 등록.
        if(GameManager.Instance.Data._isDataInitialized) SetupEnemy();
        else GameManager.Instance.Data.OnDataInitialized += SetupEnemy;
    }

    // 물리 연산용 메서드
    protected virtual void FixedUpdate()
    {
        // pending 값을 이용해 실제 이동
        if(pendingMoveDirection != Vector3.zero)
        {
            rb.MovePosition(rb.position + pendingMoveDirection * pendingMoveSpeed * Time.fixedDeltaTime);
        }
    }

    // 자식 객체는 이 메서드를 오버라이드하여 EnemyData를 로드하고, 현재 체력을 최대 체력으로 초기화 할 수 있다.
    protected virtual void SetupEnemy()
    {
        // EnemyData를 로드하고 현재 체력을 최대 체력으로 초기화
        LoadEnemyData(unitId);
        currentHp = Stat.MaxHp;

        GameManager.Instance.Data.OnDataInitialized -= SetupEnemy;
    }

    // IDamagable 인터페이스 구현
    public virtual void TakeDamage(float value)
    {
        // 데미지 계산
        currentHp -= value;

        // 이벤트 호출
        takeDamageEvent?.Invoke(currentHp);
    }

    // 이동 로직
    // 실제로 움직이지 않고, 의도를 기록
    // 실제 이동은 FixedUpdate에서 수행함.
    public void MoveToward(Vector3 targetPoistion, float moveSpeed)
    {
        pendingMoveDirection = (targetPoistion - transform.position).normalized;
        pendingMoveDirection.y = 0f; // 수평 이동

        pendingMoveSpeed = moveSpeed;  
    }

    public void StopMoving()
    {
        pendingMoveDirection = Vector3.zero;
    }


    // 디버깅용 범위 표시
#if UNITY_EDITOR
    private void OnDrawGizmos()
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
