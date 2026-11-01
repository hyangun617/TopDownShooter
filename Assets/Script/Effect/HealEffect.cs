using System;
using UnityEngine;

[Serializable]
public class HealEffect : EffectBase
{
    [SerializeField] private float amount = 10f;
    protected override void OnAdd(GameObject target)
    {
        Debug.Log($"[HealEffect] {target.name} 에게 HealEffect : {amount} 적용");

        if(Target.TryGetComponent(out UnitHealth health))
        {
            health.Heal(amount);
        }
    }
}