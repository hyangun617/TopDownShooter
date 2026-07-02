using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Enemy : UnitBase, IDamagable
{
    // 기본 스텟 데이터
    [Header("Enemy Data")]
    [SerializeField] protected EnemyData Stat;

    // 데이터 값을 지정할 Id;
    [SerializeField] protected int unitId;

    // 런타임 멤버 값.
    [Header("Runtime Data")]
    [SerializeField] protected float currentHp;

    // 피격시 호출 할 이벤트.
    public Action<float> takeDamageEvent;

    // 판정을 위한 레이어 마스크
    [Header("Target Layer Mask")]
    [SerializeField] protected LayerMask targetLayerMask;
    [SerializeField] protected LayerMask obstacleLayerMask;
    [SerializeField] protected Transform target;

    // 디버깅용 멤버
    public Color bcolor = Color.green;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // GameManager의 DataManager가 초기화 되었는지 확인하고, 초기화가 완료되었으면 SetupEnemy()를 호출. 
        // 아니면 OnDataInitialized 이벤트에 SetupEnemy()를 등록.
        if(GameManager.Instance.Data._isDataInitialized) SetupEnemy();
        else GameManager.Instance.Data.OnDataInitialized += SetupEnemy;
    }

    // 자식 객체는 이 메서드를 오버라이드하여 EnemyData를 로드하고, 현재 체력을 최대 체력으로 초기화 할 수 있다.
    protected virtual void SetupEnemy()
    {
        // EnemyData를 로드하고 현재 체력을 최대 체력으로 초기화
        LoadEnemyData(unitId);
        currentHp = Stat.MaxHp;

        GameManager.Instance.Data.OnDataInitialized -= SetupEnemy;
    }

    void LoadEnemyData(int id)
    {
        Stat = GameManager.Instance.Data.enemyTB.GetEnemyDataById(id);
    }

    // IDamagable 인터페이스 구현
    public virtual void TakeDamage(float value)
    {
        // 데미지 계산
        currentHp -= value;

        // 이벤트 호출
        takeDamageEvent?.Invoke(currentHp);
    }

    // 디버깅용 범위 표시
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 선 색상 지정
        Handles.color = bcolor;

        // 원 그리기
        Handles.DrawWireDisc(transform.position, Vector3.up, Stat.DetectRange);        
    }
#endif
}
