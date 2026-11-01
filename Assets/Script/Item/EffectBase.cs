using UnityEngine;
using System;

[Serializable]
public abstract class EffectBase
{
    [SerializeField] private float duration;            // 0 이면 즉발
    [SerializeField] private float tickInterval;        // 지속 효과 적용 주기

    [NonSerialized] private float tickTimer = 0f;       // 주기 계산용.
    [NonSerialized] private float remain;               // 남은 지속 시간

    // 목적 대상
    [NonSerialized] private GameObject target;          // 적용 대상.

    public float Duration => duration;
    public GameObject Target => target;

    public void Begin(GameObject target)
    {
        this.target = target;
        remain = duration;
        tickTimer = 0f;
        OnAdd(target);
    }

    public void Apply(GameObject target)
    {
        var instance = Clone();

        if(duration <= 0f)
        {
            instance.Begin(target);
        }
        else if(target.TryGetComponent(out EffectController ec))
        {
            ec.Add(instance, target);
        }
        else
        {
            Debug.LogWarning($"[EffectBase] {target.name}에 EffectController가 존재하지 않아 {GetType().Name} 효과 적용 실패");
        }
    }

    public bool Tick(float dt)
    {
        if(tickInterval > 0)
        {
            tickTimer += dt;
            while(tickTimer >= tickInterval)
            {
                tickTimer -= tickInterval;
                ApplyEffect();
            }    
        }

        remain -= dt;
        return remain <= 0f;        // 효과가 끝나면 true;
    }

    public EffectBase Clone() => (EffectBase)MemberwiseClone();     // 얕은 복사, 값 필드면 충분.

    // 효과 적용,
    // Begin()에서 호출 됨. 효과 시작 시 1회 실행.
    protected virtual void OnAdd(GameObject target) { }

    // 효과 적용 중,
    // 
    protected virtual void ApplyEffect() { }

    // 효과 제거 시,
    public virtual void OnStop() { }
}
    
