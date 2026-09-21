using UnityEngine;

public class UnitRangeAttack : UnitAttack
{
    public float AttackSpeed = 2f;

    // 공격 시작 지점
    public Transform firePoint;

    // 탄환 프리펩
    private GameObject bulletPrefab;
    private BulletData bulletData;

    public void Initialize(float attackSpeed)
    {
        // 풀링으로 OnSpawn 이 반복 호출되므로 ScriptableObject 는 한 번만 생성.
        if (bulletData == null)
            bulletData = ScriptableObject.CreateInstance<BulletData>();

        bulletPrefab = GameManager.Instance.DataMgr.Get<GameObject>("Bullet");
        AttackSpeed = attackSpeed;
    }

    // 공격 실행 메서드. (애니메이션 이벤트에서 직접 호출됨)
    public override void PlayAttack()
    {
        if (bulletPrefab == null) return;

        StartCooldown();

        // 탄환 정보 초기화
        bulletData.damage = AttackDamage;
        bulletData.range = AttackRange;
        bulletData.speed = AttackSpeed;
        bulletData.Piercing = false;

        GameObject bulletObj = GameManager.Instance.PoolMgr.Get(bulletPrefab);
        bulletObj.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);

        if (bulletObj.TryGetComponent<Bullet>(out var spawnedBullet))
        {
            PlayAttackSfx(followTarget: transform, clipVolume: 0.5f, pitch: 1.5f);
            spawnedBullet.ShootBullet(bulletData, firePoint.position, transform.forward.normalized, TargetLayerMask);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!IsCoolingDown || firePoint == null) return;

        Gizmos.color = Color.red;
        Vector3 start = firePoint.position;
        Gizmos.DrawLine(start, start + transform.forward * AttackRange);
    }
#endif
}
