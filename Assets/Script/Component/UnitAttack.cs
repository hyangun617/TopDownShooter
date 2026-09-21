using System.Collections.Generic;
using UnityEngine;

// 모든 유닛 공격 컴포넌트(근접 / 원거리 / 플레이어 무기)의 공통 베이스.
public abstract class UnitAttack : MonoBehaviour, IAttackable
{
    // IAttackable
    public float AttackDamage { get; set; }
    public float AttackRange { get; set; }
    public float AttackDelay { get; set; }

    // 공격 대상 레이어 마스크
    public LayerMask TargetLayerMask { get; set; }

    // 공격 후 다음 공격까지의 대기 상태.
    private float nextAttackTime;
    public bool IsCoolingDown => Time.time < nextAttackTime;

    // 공격 SFX
    private List<AudioClip> attackSFX;

    public abstract void PlayAttack();

    public void SetAttackSFX(List<AudioClip> clips) => attackSFX = clips;

    // 공격 후 AttackDelay 동안 IsCoolingDown 이 true 가 된다.
    protected void StartCooldown() => nextAttackTime = Time.time + AttackDelay;

    protected void PlayAttackSfx(Vector3? worldPosition = null, Transform followTarget = null, float clipVolume = 1f, float pitch = 1f)
    {
        GameManager.Instance.SoundMgr.PlaySfx(attackSFX.PickRandom(), worldPosition, clipVolume, followTarget, pitch);
    }
}
