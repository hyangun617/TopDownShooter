using System;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimController), typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    // 플레이어 객체
    private Rigidbody rb;              
    private Vector2 moveInput;      
    private PlayerAnimController playerAnimController;
    private PlayerAttack playerAttack;
    private WeaponManager weaponManager;

    [SerializeField] private bool isOnReload = false;
    [SerializeField] private bool isAbleReload = false;

    private float moveSpeed;

    void OnEnable()
    {
        playerAttack.isAmmoZero += OnReload;
        playerAttack.OnReloadComplete += OnReloadComplete;
        playerAttack.OnReloadFailed += OnReloadFail;
        playerAttack.OnAmmoChanged += OnAbleReload;
        InputManager.Instance.OnMove += OnMove;
        InputManager.Instance.OnPressed_R += OnReload;
    }

    void OnDisable()
    {
        playerAttack.isAmmoZero -= OnReload;
        playerAttack.OnReloadComplete -= OnReloadComplete;
        playerAttack.OnReloadFailed -= OnReloadFail;
        playerAttack.OnAmmoChanged -= OnAbleReload;
        InputManager.Instance.OnMove -= OnMove;
        InputManager.Instance.OnPressed_R -= OnReload;
    }

    private void Awake()
    {
        // GameObject에 등록된 컴포넌트를 받아옴.
        rb = GetComponent<Rigidbody>();
        playerAnimController = GetComponent<PlayerAnimController>();
        playerAttack = GetComponent<PlayerAttack>();
        weaponManager = GetComponent<WeaponManager>();
    }

    // 초기값을 받아오는 메서드
    public void Init(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(InputManager.Instance == null ) return;
    }

    // Update is called once per frame
    void Update()
    {
        // 컨트롤러가 활성화 되어 있을 때만 회전.
        if (InputManager.Instance != null && InputManager.Instance.IsInputEnabled)
        {
            LookAtMouse();

            // 로컬 좌표계 전환
            // 캐릭터가 바라보는 기준으로한 로컬 좌표계
            Vector3 localMove = transform.InverseTransformDirection(new Vector3(moveInput.x, 0, moveInput.y));
            bool isMoved = moveInput.magnitude > 0;

            playerAnimController.UpdateMoveParams(localMove, moveSpeed / 10f, isMoved);
        }        
    }

    private void FixedUpdate()
    {
        // 입력 값에 따라 단위 벡터를 받아옴.
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    private void LookAtMouse()
    {
        Vector3 targetPos = InputManager.Instance.mouseWorldPos;
        Vector3 Direction = targetPos - rb.position;
        // y 좌표 값은 무시.
        Direction.y = 0f;

        if(Direction != Vector3.zero)
        {
            // 마우스를 향해 회전.
            rb.rotation = Quaternion.LookRotation(Direction);
        }
    }

    private void OnAbleReload(int ammo)
    {
        if(ammo < weaponManager.WeaponData.magazineSize)
        {
            isAbleReload = true;
        }
        else
        {
            isAbleReload = false;
        }
    }

    private void OnReload()
    {
        if(!isAbleReload) return;           // 재장전 가능 여부 확인
        if(isOnReload) return;              // 재장전 중인지 확인
        isOnReload = true;

        // 리로드 메서드
        float animLength = playerAnimController.OnReload();      
        float SfxLength = weaponManager.WeaponData.reloadSFX.length;
        float pitch = Mathf.Clamp(SfxLength / animLength, 0.8f, 1.5f);
        GameManager.Instance.SoundMgr.PlaySfx(weaponManager.WeaponData.reloadSFX, followTarget: this.transform, pitch: pitch);
        playerAttack.TryReload();
    }

    private void OnReloadComplete(int val)
    {
        isOnReload = false;
        isAbleReload = false;
    }

    private void OnReloadFail()
    {
        isOnReload = false;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.OnMove -= OnMove;
        InputManager.Instance.OnPressed_R -= OnReload;
    }

    void OnMove(Vector2 input)
    {
        if (InputManager.Instance.IsInputEnabled)
        {
            moveInput = input;
        }        
    }
}
