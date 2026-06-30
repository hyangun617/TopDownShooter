using System;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class Enemy : UnitBase, IDamagable
{
    // 기본 스텟 데이터
    [SerializeField] protected EnemyData Stat;

    // 데이터 값을 지정할 Id;
    [SerializeField] private int unitId;

    // 런타임 멤버 값.
    [SerializeField] private float currentHp;

    // 피격시 호출 할 이벤트.
    public Action<float> takeDamageEvent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // GameManager의 DataManager가 초기화 되었는지 확인하고, 초기화가 완료되었으면 SetupEnemy()를 호출. 
        // 아니면 OnDataInitialized 이벤트에 SetupEnemy()를 등록.
        if(GameManager.Instance.Data._isDataInitialized) SetupEnemy();
        else GameManager.Instance.Data.OnDataInitialized += SetupEnemy;
    }

    void SetupEnemy()
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

    public void TakeDamage(float value)
    {
        // 데미지 계산
        currentHp -= value;

        // 이벤트 호출
        takeDamageEvent?.Invoke(currentHp);
    }
}
