using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField] private float damage = 10f;                 // 기본 데미지
    [SerializeField] private float fireRate = 0.2f;                 // 사격 딜레이
    [SerializeField] private float range = 50f;                    // 사거리.

    private Camera _cam;                                           // 카메라를 기준으로 마우스의 위치를 읽어오기 때문에 카메라 객체를 읽어와야함.
    private int groundLayer;                                        // Physics.Raycast의 레이어 마스크를 위한 int 변수.
    private int attackableLayer;

    private InputSystem inputSystem;                             // 마우스 클릭 메서드를 등록할 InputSystem
    private bool isFiring = false;                                    // 마우스 클릭(발사) 여부;.
    private Vector2 mouseScreenPos;                              // 실시간 마우스 위치.
    private float fireCooldown = 0f;                                // 다음 발사까지 남은 시간을 추적하는 타이머.


    private void Awake()
    {
        inputSystem = new InputSystem();
        _cam = Camera.main;                                         // 메인 카메라 할당.

        // 레이어 마스크를 위해 해당 레이어가 지정된 순서를 읽어옴.
        groundLayer = LayerMask.GetMask("Ground");
        attackableLayer = LayerMask.GetMask("Attackable");
    }

    private void OnEnable()
    {
        inputSystem.Player.Enable();
        inputSystem.Player.Fire.performed += onFire;
        inputSystem.Player.Fire.canceled += onFire;
        inputSystem.Player.MovePosition.performed += onMouseMoved;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!isFiring) return;

        // 이전 프레임부터 현재 프레임 사이의 수를 이용해 쿨타임 계산.
        fireCooldown -= Time.deltaTime;

        if(fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = fireRate;
        }
    }

    private void OnDisable()
    {
        inputSystem.Player.Disable();
        inputSystem.Player.Fire.performed -= onFire;
        inputSystem.Player.Fire.canceled -= onFire;
        inputSystem.Player.MovePosition.performed -= onMouseMoved;
    }

    private void onFire(InputAction.CallbackContext ctx)
    {
        isFiring = ctx.performed;
    }

    // 마우스가 움직일 때 마다 해당 마우스의 위치를 갱신.
    private void onMouseMoved(InputAction.CallbackContext ctx)
    {
        mouseScreenPos = ctx.ReadValue<Vector2>();
    }

    private void Fire()
    {
        // Raycast
        // 카메라에서 부터 마우스의 위치까지 직선(ray)을 그음.
        Ray ray = _cam.ScreenPointToRay(mouseScreenPos);

        // 바닥에 닿은 지점의 포지션을 groundHit에 저장함.
        // Mathf.Infinity -> 무한수 -> ray는 바닥만을 감지하기에 성능에도 문제 없음.
        if (!Physics.Raycast(ray, out RaycastHit groundHit, Mathf.Infinity, groundLayer)) return;

        Debug.Log(groundHit.point);

        // 현재 플레이어의 위치부터 groundHit까지의 벡터를 단위 벡터화 시킴.
        // 방향만을 남기기 위함.
        Vector3 direction = (groundHit.point - transform.position).normalized;

        // 플레이어의 위치에서 단위 벡터 direction의 방향으로 range 만큼의 사거리로 ray 발사. -> attackableLayer의 레이어만 감지함.
        if(Physics.Raycast(transform.position, direction, out RaycastHit otherHit, range, attackableLayer))
        {
            // 상대방 객체의 충돌체를 읽어와 오브젝트를 감지함.


        }
    }
}
