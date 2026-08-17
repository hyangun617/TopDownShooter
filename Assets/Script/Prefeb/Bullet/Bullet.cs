using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    public GameObject SourcePrefab { get; set; }

    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float lifeTime;
    [SerializeField] private LayerMask target;
    private Vector3 direction;
    private Vector3 prevPos;

    private bool isActivated = false;

    public void OnSpawn()
    {
        
    }

    void Update()
    {
        // 활성화 상태가 아니라면 동작하지 않음.
        if(!isActivated) return;

        // 사거리를 초과하면 반환.
        if(lifeTime <= 0f)
        {
            ResetBullet();
            return;
        }
        
        prevPos = transform.position;
        transform.position += direction * speed * Time.deltaTime;

        if(Physics.Linecast(prevPos, transform.position, out RaycastHit hit, target))
        {
            if(hit.collider.TryGetComponent<IDamagable>(out var hitObject))
            {
                hitObject.TakeDamage(damage);
                ResetBullet();
            }

            return;
        }

        lifeTime -= Time.deltaTime;
    }

    // 탄환 발사 메소드
    public void ShootBullet(BulletData bulletData, Vector3 point, Vector3 dir, LayerMask target)
    {
        damage = bulletData.damage;
        speed = bulletData.speed;
        range = bulletData.range;
        this.target = target;

        lifeTime = range / speed;
        direction = dir;

        // 위치 옮김.
        gameObject.transform.position = point;
        prevPos = point;
        isActivated = true;
        gameObject.SetActive(true);
    }

    // 충돌 판정 이후 초기화.
    private void ResetBullet()
    {
        damage = 0f;
        speed = 0f;
        range = 0f;
        target = default;
        direction = Vector3.zero;
        prevPos = Vector3.zero;
        lifeTime = 0f;
        isActivated = false;
        gameObject.SetActive(false);

        // 이후 풀로 반환 로직.
        GameManager.Instance.PoolMgr.Release(gameObject);
    }

    public void OnDespawn()
    {
        
    }
}
