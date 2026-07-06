using System;
using UnityEngine;

public class UnitHealth : MonoBehaviour, IDamagable 
{
    private float currentHp;
    private float maxHp;

    public float CurrentHp => currentHp;
    public bool IsDead => currentHp <= 0;

    public event Action<float> OnDamaged;           // 데미지를 입었을 때 이벤트
    public event Action OnDeath;                    // 사망 시점 : 한번만 호출됨.

    private bool isDead = false;

    public void Initialize(float maxHp)
    {
        this.maxHp = maxHp;
        currentHp = maxHp;
        isDead = false;

        // 오브젝트 풀링 시 이전 구독자가 남아있지 않도록 초기화.
        OnDamaged = null;
        OnDeath = null;
    }

    public void TakeDamage(float value)
    {
        if (isDead) return;         // 중복 사망 처리 방지

        currentHp -= value;
        OnDamaged?.Invoke(currentHp);

        if(currentHp <= 0)
        {
            isDead = true;
            OnDeath?.Invoke();      // 사망 이벤트 발생.
        }
    }
}