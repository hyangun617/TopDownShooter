using UnityEngine;
using System.Collections.Generic;

public class EffectController : MonoBehaviour
{
    readonly List<EffectBase> effects = new();

    void Update()
    {
        float dt = Time.deltaTime;
        for(int i = effects.Count - 1; i >= 0; --i)
        {
            // 효과가 끝난 경우 Tick()에서 true 반환.
            if(effects[i].Tick(dt))
            {
                effects[i].OnStop();
                effects.RemoveAt(i);
            }
        }
    }

    public void Add(EffectBase effect, GameObject target)
    {
        effect.Begin(target);
        effects.Add(effect);
    }
}