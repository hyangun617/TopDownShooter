using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    // SFX
    [SerializeField] private AudioClip attackSFX;

    void Start()
    {
        Addressables.LoadAssetAsync<GameObject>("Bullet").Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                bulletPrefeb = handle.Result;
                prefabLoaded = true;
            }
        };

        bulletData = ScriptableObject.CreateInstance<BulletData>();
    }

    public void PlayAttack()
    {
        if(!prefabLoaded) return;

        // 탄환 정보 초기화
        bulletData.damage = AttackDamage;
        bulletData.range = AttackRange;
        bulletData.speed = AttackSpeed;
        bulletData.Piercing = false;

        // 원거리 공격 메서드
        StartCoroutine(OnRangeAttacking());
        Debug.Log("Range Attack!");

        Bullet spawnedBullet = Instantiate<Bullet>(bulletPrefeb.GetComponent<Bullet>(), firePoint.position, firePoint.rotation);
        spawnedBullet.gameObject.SetActive(false);
        
        // 발사 방향
        Vector3 dir = transform.forward.normalized; 

        // 발사.
        spawnedBullet.ShootBullet(bulletData, firePoint.position, dir, targetLayerMask);           
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