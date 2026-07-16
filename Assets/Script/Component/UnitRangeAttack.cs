using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitRangeAttack : MonoBehaviour, IAttackable
{
    // IAttackable 인터페이스 멤버 
    public float AttackDamage { get; set; }
    public float AttackRange { get; set; }
    public float AttackDelay { get; set; }
    public float AttackSpeed = 2f;

    // 공격 시작 지점
    public Transform firePoint;

    // 공격 대상 레이어 마스크
    public LayerMask targetLayerMask;

    // 탄환 프리펩
    private GameObject bulletPrefeb;
    private BulletData bulletData;
    private bool prefabLoaded = false;

    // 공격 여부
    private bool isAttacking = false;
    private float isCooldown;

    // SFX
    private List<AudioClip> attackSFX;

    public void Initialize()
    {
        bulletData = ScriptableObject.CreateInstance<BulletData>();

        bulletPrefeb = GameManager.Instance.DataMgr.bulletPrefab;
        prefabLoaded = bulletPrefeb != null;

        isCooldown = AttackDelay;
    }

    void Update()
    {
        if(isCooldown >= 0) isCooldown -= Time.deltaTime;
    }

    public void PlayAttack()
    {
        if(!prefabLoaded) return;
        if(isCooldown >= 0) return;

        // 탄환 정보 초기화
        bulletData.damage = AttackDamage;
        bulletData.range = AttackRange;
        bulletData.speed = AttackSpeed;
        bulletData.Piercing = false;

        // 원거리 공격 메서드
        StartCoroutine(OnRangeAttacking());
        Debug.Log("Range Attack!");
        
        // 발사 방향
        Vector3 dir = transform.forward.normalized; 

        GameObject bulletObj = GameManager.Instance.PoolMgr.Get(bulletPrefeb);

        bulletObj.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        Debug.Log($"Bullet Position is : {firePoint.position}");

        if(bulletObj.TryGetComponent<Bullet>(out var spawnedBullet))
        {
            spawnedBullet.ShootBullet(bulletData, firePoint.position, dir, targetLayerMask);
            isCooldown = AttackDelay;   
        }  
    }

    private IEnumerator OnRangeAttacking()
    {
        isAttacking = true;
        yield return new WaitForSeconds(AttackDelay);
        isAttacking = false;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(isAttacking)
        {
            Gizmos.color = Color.red;
            Vector3 start = firePoint.position;
            Vector3 end = start + transform.forward * AttackRange;
            Gizmos.DrawLine(start, end);    
        }        
    }
#endif
}