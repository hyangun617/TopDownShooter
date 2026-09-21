using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float currentHp;
    private float maxHp;
    private bool isDead;

    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;
    public bool IsDead => isDead;

    public event Action<float> OnDamaged;               // 데미지를 입었을 때 : 현재 체력
    public event Action<float, float> OnHpChanged;      // 체력이 변했을 때(피해 / 회복) : 현재 체력, 최대 체력
    public event Action OnDeath;                        // 사망 시점 : 한번만 호출됨.

    // SFX
    private List<AudioClip> damagedSFX;
    private List<AudioClip> deathSFX;

    public void SetDamageSFX(List<AudioClip> damagedSFX) => this.damagedSFX = damagedSFX;
    public void SetDeathSFX(List<AudioClip> deathSFX) => this.deathSFX = deathSFX;

    public void Initialize(float maxHp)
    {
        this.maxHp = maxHp;
        currentHp = maxHp;
        isDead = false;

        // 오브젝트 풀링 시 이전 구독자가 남아있지 않도록 초기화.
        OnDamaged = null;
        OnHpChanged = null;
        OnDeath = null;
    }

    public void TakeDamage(float value)
    {
        if (isDead) return;         // 중복 사망 처리 방지

        currentHp -= value;
        PlaySfx(damagedSFX);

        OnDamaged?.Invoke(currentHp);
        OnHpChanged?.Invoke(currentHp, maxHp);

        if (currentHp <= 0)
        {
            isDead = true;
            PlaySfx(deathSFX);
            OnDeath?.Invoke();
        }
    }

    public void Heal(float value)
    {
        if (isDead) return;

        currentHp = Mathf.Min(currentHp + value, maxHp);
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    private void PlaySfx(List<AudioClip> clips)
    {
        GameManager.Instance.SoundMgr.PlaySfx(clips.PickRandom(), clipVolume: 0.5f, followTarget: transform);
    }
}
