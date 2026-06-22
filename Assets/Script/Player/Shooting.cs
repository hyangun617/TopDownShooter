using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [Header("기본 설정 (Defaults)")]
    [SerializeField] private float damage = 10f;                 // 기본 데미지
    [SerializeField] private float fireRate = 0.2f;                 // 사격 딜레이
    [SerializeField] private float range = 50f;                    // 사거리.

    [Header("시각 효과 (Visuals)")]
    [SerializeField] private Transform firePoint;                // 총구 위치
    [SerializeField] private LineRenderer bulletTrail;              // 궤적을 그릴 렌더러

    private LayerMask attackableLayer;


    private float fireCooldown = 0f;                               // 다음 발사까지 남은 시간을 추적하는 타이머.
    private bool isFiring = false;                                   // 발사 여부.

    private void Awake()
    {
        attackableLayer = LayerMask.GetMask("Attackable");
    }

    private void OnEnable()
    {
        
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
        if(fireCooldown >= 0) fireCooldown -= Time.deltaTime;

        if (isFiring && fireCooldown <= 0)  
            Fire();
    }

    private void OnDisable()
    {

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


    private void Fire()
    {
        // input Manager에서 마우스의 위치를 받아옴.
        Vector3 ClickPoint = InputManager.Instance.mouseWorldPos;

        // 현재 플레이어의 위치부터 클릭 위치까지의 벡터를 단위 벡터화 시킴.
        // 방향만을 남기기 위함.
        Vector3 direction = (ClickPoint - firePoint.position).normalized;

        // 궤적 활성화
        bulletTrail.enabled = true;
        bulletTrail.SetPosition(0, firePoint.position);

        // 플레이어의 위치에서 단위 벡터 direction의 방향으로 range 만큼의 사거리로 ray 발사. -> attackableLayer의 레이어만 감지함.
        if(Physics.Raycast(transform.position, direction, out RaycastHit otherHit, range, attackableLayer))
        {
            // 상대방 객체의 충돌체를 읽어와 오브젝트를 감지함.

            bulletTrail.SetPosition(1, otherHit.point);

            Debug.Log($"{firePoint.position}, {otherHit.point} FIRE!");
        }
        else // 아무도 맞지 않은 경우
        {
            Vector3 endPoint = firePoint.position + direction * range;
            bulletTrail.SetPosition(1, endPoint);

            Debug.Log($"{firePoint.position}, {endPoint} FIRE!");
        }

        // 코루틴을 사용하여 0.05초만 라인이 보이도록 함.
        // 이전에 이미 실행된 코루틴이 있다면, 종료하고 실행함.
        StopCoroutine(nameof(FlashBulletTrail));        
        StartCoroutine(nameof(FlashBulletTrail));

        // 디버그 전용, 씬에서만 해당 라인이 보임.
        Debug.DrawLine(firePoint.position, bulletTrail.GetPosition(1), Color.red, 0.5f);
        
        fireCooldown = fireRate;
    }

    private IEnumerator FlashBulletTrail()
    {
        bulletTrail.enabled = true;
        yield return new WaitForSeconds(0.05f);
        bulletTrail.enabled = false;
    }
}
