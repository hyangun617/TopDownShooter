using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 무기 공격(히트스캔) + 탄약 / 재장전 관리.
[RequireComponent(typeof(PlayerAnimController))]
public class PlayerAttack : UnitAttack
{
    [Header("시각 효과 (Visuals)")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer bulletTrail;

    private PlayerAnimController animController;
    private WeaponData weaponData;

    private int currentAmmo;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => weaponData != null ? weaponData.magazineSize : 0;

    private bool isReloading;
    private bool isFireRequested;

    public event Action isAmmoZero;                     // 장탄 0
    public event Action<int> OnAmmoChanged;             // 장탄 상황 변경   int = 장탄 값
    public event Action OnReloadStart;                  // 재장전 시작
    public event Action<float> OnReloadProgress;        // 재장전 중        float = 진행도
    public event Action OnReloadFailed;                 // 재장전 실패
    public event Action<int> OnReloadComplete;          // 재장전 완료      int = 장탄 값

    private Coroutine reloadCoroutine;
    private Coroutine flashRoutine;

    private const float TrailDuration = 0.05f;

    private void Awake()
    {
        TargetLayerMask = LayerMask.GetMask("Attackable");
        animController = GetComponent<PlayerAnimController>();
    }

    private void Start()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnFire += OnFire;

        if (firePoint == null)
            firePoint = transform;
    }

    private void Update()
    {
        if (isFireRequested && !IsCoolingDown && !isReloading && weaponData != null)
        {
            PlayAttack();
            animController.AttackTrigger();
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnFire -= OnFire;
    }

    private void OnFire(FireEventArgs args)
    {
        isFireRequested = args.IsPressed;
    }

    #region Reload

    public void TryReload()
    {
        if (weaponData == null ||
            currentAmmo >= weaponData.magazineSize ||
            isReloading)
        {
            OnReloadFailed?.Invoke();
            return;
        }

        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);

        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        OnReloadStart?.Invoke();

        float elapsed = 0f;
        float reloadTime = weaponData.reloadTime;

        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            OnReloadProgress?.Invoke(Mathf.Clamp01(elapsed / reloadTime));
            yield return null;
        }

        currentAmmo = weaponData.magazineSize;
        isReloading = false;
        reloadCoroutine = null;

        OnAmmoChanged?.Invoke(currentAmmo);
        OnReloadComplete?.Invoke(currentAmmo);
    }

    public void CancelReload()
    {
        if (reloadCoroutine == null) return;

        StopCoroutine(reloadCoroutine);
        reloadCoroutine = null;
        isReloading = false;
        OnReloadFailed?.Invoke();
    }

    #endregion

    public void SetFirePoint(Transform newFirePoint) => firePoint = newFirePoint;

    public void SetWeaponData(WeaponData weaponData)
    {
        this.weaponData = weaponData;
        AttackRange = weaponData.range;
        AttackDelay = weaponData.fireRate;
        AttackDamage = weaponData.damage;
        SetAttackSFX(new List<AudioClip> { weaponData.fireSFX });
        currentAmmo = weaponData.magazineSize;

        OnAmmoChanged?.Invoke(currentAmmo);
    }

    private bool CheckAttackAvailable()
    {
        if (weaponData == null || isReloading)
            return false;

        if (currentAmmo <= 0)
        {
            isAmmoZero?.Invoke();
            return false;
        }

        return true;
    }

    public override void PlayAttack()
    {
        if (!CheckAttackAvailable())
        {
            OnReloadFailed?.Invoke();
            return;
        }

        currentAmmo--;

        // 발사 방향 : 마우스 위치를 향해 수평으로.
        Vector3 clickPoint = InputManager.Instance.mouseWorldPos;
        clickPoint.y = 0f;

        Vector3 firePosition = firePoint.position;
        firePosition.y = 0f;

        Vector3 direction = (clickPoint - firePosition).normalized;

        PlayAttackSfx(worldPosition: firePoint.position);

        // 히트스캔 판정
        Vector3 endPoint = firePoint.position + direction * AttackRange;
        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, AttackRange, TargetLayerMask))
        {
            endPoint = firePoint.position + direction * hit.distance;

            if (hit.collider.TryGetComponent<IDamagable>(out var target))
                target.TakeDamage(AttackDamage);
        }

        ShowBulletTrail(endPoint);
        Debug.DrawLine(firePoint.position, endPoint, Color.red, 0.5f);

        StartCooldown();
        OnAmmoChanged?.Invoke(currentAmmo);
    }

    #region Visual

    private void ShowBulletTrail(Vector3 endPoint)
    {
        if (bulletTrail == null) return;

        bulletTrail.SetPosition(0, firePoint.position);
        bulletTrail.SetPosition(1, endPoint);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashBulletTrail());
    }

    private IEnumerator FlashBulletTrail()
    {
        bulletTrail.enabled = true;
        yield return new WaitForSeconds(TrailDuration);
        bulletTrail.enabled = false;
    }

    #endregion
}
