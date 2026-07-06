using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour, IAttackable
{
    [Header("시각 효과 (Visuals)")]
    [SerializeField] private Transform firePoint;                  // 총구 위치
    [SerializeField] private LineRenderer bulletTrail;             // 궤적을 그릴 렌더러

    private LayerMask attackableLayer;                              // 공격 가능 객체 필터링 레이어 마스크
    
    // IAttackable
    public float AttackRange { get; set; }
    public float AttackDelay { get; set; }
    public float AttackDamage { get; set; }

    private float attackCooldown = 0f;                              // 공격 쿨다운.

    private bool isFiring = false;                                  // 발사 여부.

    // SFX
    [SerializeField] private AudioClip AttackSFX;

    private void Awake()
    {
        attackableLayer = LayerMask.GetMask("Attackable");
    }

    void Start()
    {
        // 입력에 발사 함수 등록
        InputManager.Instance.OnFire += OnFire;

        // 기본 값을 입력하지 않았다면 기본 값 설정.
        if (firePoint == null)
            firePoint = this.transform;
    }

    void Update()
    {
        // 이전 프레임부터 현재 프레임 사이의 수를 이용해 쿨타임 계산.
        if(attackCooldown >= 0) attackCooldown -= Time.deltaTime;

        if (isFiring && attackCooldown <= 0)  
            PlayAttack();
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnFire -= OnFire;
    }

    private void OnFire(FireEventArgs args)
    {
        if (args.IsPressed)
            isFiring = true;
        else
        {
            isFiring = false;

            // 몇 초 이상 눌렀는지 args 값의 holdDuration 값을 이용해 계산함.

        }
    }


    public void PlayAttack()
    {
        // Top-Down 이기 때문에 Y축 값은 무시함. 
        // input Manager에서 마우스의 위치를 받아옴.
        Vector3 ClickPoint = new Vector3(InputManager.Instance.mouseWorldPos.x, 0f, InputManager.Instance.mouseWorldPos.z);

        // fixed fire Postion
        Vector3 fixedFirePosition = new Vector3(firePoint.position.x, 0f, firePoint.transform.position.z);

        // 현재 플레이어의 위치부터 클릭 위치까지의 벡터를 단위 벡터화 시킴.
        // 방향만을 남기기 위함.
        Vector3 direction = (ClickPoint - fixedFirePosition).normalized;

        // 효과음 재생
        GameManager.Instance.Sound.PlaySfx(AttackSFX);

        // 궤적 활성화
        bulletTrail.enabled = true;
        bulletTrail.SetPosition(0, firePoint.transform.position);

        // 플레이어의 위치에서 단위 벡터 direction의 방향으로 range 만큼의 사거리로 ray 발사. -> attackableLayer의 레이어만 감지함.
        if(Physics.Raycast(firePoint.transform.position, direction, out RaycastHit otherHit, AttackRange, attackableLayer))
        {
            Debug.Log($"Hit Object: {otherHit.collider.gameObject.name}, Layer: {LayerMask.LayerToName(otherHit.collider.gameObject.layer)}");

            // 상대방 객체의 충돌체를 읽어와 오브젝트를 감지함.
            Vector3 lastPoint = new Vector3(otherHit.point.x, firePoint.transform.position.y, otherHit.point.z);
            bulletTrail.SetPosition(1, lastPoint);

            if(otherHit.collider.TryGetComponent<IDamagable>(out var enemy))
            {
                enemy.TakeDamage(AttackDamage);
            }
        }
        else // 아무도 맞지 않은 경우
        {
            Vector3 endPoint = fixedFirePosition + direction * AttackRange;
            bulletTrail.SetPosition(1, endPoint);

            Debug.Log($"{firePoint.transform.position}, {endPoint} FIRE!");
        }

        // 코루틴을 사용하여 0.05초만 라인이 보이도록 함.
        // 이전에 이미 실행된 코루틴이 있다면, 종료하고 실행함.
        StopCoroutine(nameof(FlashBulletTrail));        
        StartCoroutine(nameof(FlashBulletTrail));

        // 디버그 전용, 씬에서만 해당 라인이 보임.
        Debug.DrawLine(firePoint.transform.position, bulletTrail.GetPosition(1), Color.red, 0.5f);
        
        attackCooldown = AttackDelay;
    }

    private IEnumerator FlashBulletTrail()
    {
        bulletTrail.enabled = true;
        yield return new WaitForSeconds(0.05f);
        bulletTrail.enabled = false;
    }
}
