using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimController))]
public class PlayerAttack : MonoBehaviour, IAttackable
{
    [Header("시각 효과 (Visuals)")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer bulletTrail;

    private LayerMask attackableLayer;
    private PlayerAnimController animController;
    private WeaponData weaponData;

    public float AttackRange { get; set; }
    public float AttackDelay { get; set; }
    public float AttackDamage { get; set; }

    private int currentAmmo;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => weaponData != null ? weaponData.magazineSize : 0;

    private bool isReloading;
    private float attackCooldown;
    private bool isFireRequested;

    public event Action isAmmoZero;                     // 장탄 0
    public event Action<int> OnAmmoChanged;             // 장탄 상황 변경   int = 장탄 값
    public event Action OnReloadStart;                  // 재장전 시작
    public event Action<float> OnReloadProgress;        // 재장전 중        float = 진행도
    public event Action OnReloadFailed;                 // 재장전 실패
    public event Action<int> OnReloadComplete;          // 재장전 완료      int = 장탄 값

    private Coroutine reloadCoroutine;
    private Coroutine flashRoutine;

    private AudioClip attackSfx;

    private void Awake()
    {
        attackableLayer = LayerMask.GetMask("Attackable");
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
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        if (isFireRequested && attackCooldown <= 0f && !isReloading && weaponData != null)
        {
            PlayAttack();
            animController.OnShoot();
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

    public void TryReload()
    {
        if (weaponData == null || 
            currentAmmo >= weaponData.magazineSize ||
            isReloading)
        {   
            // reload 실패
            OnReloadFailed?.Invoke();
            return;
        }

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }
            

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
        NotifyAmmoChange();
        OnReloadComplete?.Invoke(currentAmmo);
        OnAmmoChanged?.Invoke(currentAmmo);
    }

    private void NotifyAmmoChange()
    {
        OnAmmoChanged?.Invoke(currentAmmo);
    }

    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            OnReloadFailed?.Invoke();
            isReloading = false;
        }
    }

    public void SetFirePoint(Transform newFirePoint) => firePoint = newFirePoint;

    public void SetWeaponData(WeaponData weaponData)
    {
        this.weaponData = weaponData;
        AttackRange = weaponData.range;
        AttackDelay = weaponData.fireRate;
        AttackDamage = weaponData.damage;
        attackSfx = weaponData.fireSFX;
        currentAmmo = weaponData.magazineSize;

        NotifyAmmoChange();
    }

    private bool CheckAttackAvailable()
    {
        if (weaponData == null || isReloading)
            return false;

        if (currentAmmo <= 0)
        {
            Debug.Log("is Ammo Zero Invoke");
            isAmmoZero?.Invoke();
            return false;
        }

        return true;
    }

    public void PlayAttack()
    {
        if(!CheckAttackAvailable())
        {
            OnReloadFailed?.Invoke();
            return;
        }        

        currentAmmo--;

        Vector3 clickPoint = InputManager.Instance.mouseWorldPos;
        clickPoint.y = 0f;

        Vector3 firePosition = firePoint.position;
        firePosition.y = 0f;

        Vector3 direction = (clickPoint - firePosition).normalized;

        if (attackSfx != null)
            GameManager.Instance.SoundMgr.PlaySfx(attackSfx, worldPosition: firePoint.position);

        if (bulletTrail != null)
        {
            bulletTrail.enabled = true;
            bulletTrail.SetPosition(0, firePoint.position);
        }

        Vector3 endPoint = firePoint.position + direction * AttackRange;

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit otherHit, AttackRange, attackableLayer))
        {
            Vector3 hitPosition = firePoint.position + direction * otherHit.distance;

            if (bulletTrail != null)
                bulletTrail.SetPosition(1, hitPosition);

            if (otherHit.collider.TryGetComponent<IDamagable>(out var enemy))
                enemy.TakeDamage(AttackDamage);
        }
        else
        {
            if (bulletTrail != null)
                bulletTrail.SetPosition(1, endPoint);
        }

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashBulletTrail());

        Debug.DrawLine(firePoint.position, bulletTrail != null ? bulletTrail.GetPosition(1) : endPoint, Color.red, 0.5f);

        attackCooldown = AttackDelay;
        NotifyAmmoChange();
    }

    private IEnumerator FlashBulletTrail()
    {
        if (bulletTrail == null)
            yield break;

        bulletTrail.enabled = true;
        yield return new WaitForSeconds(0.05f);
        bulletTrail.enabled = false;
    }
}
