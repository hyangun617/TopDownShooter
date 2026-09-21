using UnityEngine;

// 플레이어의 입력 처리 : 이동 / 조준 / 재장전.
// 실제 이동과 회전은 UnitController 에 위임한다.
[RequireComponent(typeof(UnitController), typeof(PlayerAnimController), typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    private UnitController unitController;
    private PlayerAnimController animController;
    private PlayerAttack playerAttack;
    private WeaponManager weaponManager;

    private Vector2 moveInput;
    private float moveSpeed;

    [SerializeField] private bool isOnReload = false;
    [SerializeField] private bool isAbleReload = false;

    private void Awake()
    {
        unitController = GetComponent<UnitController>();
        animController = GetComponent<PlayerAnimController>();
        playerAttack = GetComponent<PlayerAttack>();
        weaponManager = GetComponent<WeaponManager>();
    }

    // 초기값을 받아오는 메서드
    public void Initialize(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        playerAttack.isAmmoZero += OnReload;
        playerAttack.OnReloadComplete += OnReloadComplete;
        playerAttack.OnReloadFailed += OnReloadFail;
        playerAttack.OnAmmoChanged += OnAbleReload;

        if (InputManager.Instance == null) return;
        InputManager.Instance.OnMove += OnMove;
        InputManager.Instance.OnPressed_R += OnReload;
    }

    private void OnDisable()
    {
        playerAttack.isAmmoZero -= OnReload;
        playerAttack.OnReloadComplete -= OnReloadComplete;
        playerAttack.OnReloadFailed -= OnReloadFail;
        playerAttack.OnAmmoChanged -= OnAbleReload;

        // 컨트롤러가 꺼지면(사망 등) 입력이 사라지므로 이동도 멈춘다.
        unitController.StopMoving();

        if (InputManager.Instance == null) return;
        InputManager.Instance.OnMove -= OnMove;
        InputManager.Instance.OnPressed_R -= OnReload;
    }

    private void Update()
    {
        if (InputManager.Instance == null || !InputManager.Instance.IsInputEnabled) return;

        unitController.LookAt(InputManager.Instance.mouseWorldPos);

        // 캐릭터가 바라보는 기준의 로컬 좌표계로 전환
        Vector3 localMove = transform.InverseTransformDirection(new Vector3(moveInput.x, 0, moveInput.y));
        bool isMoved = moveInput.magnitude > 0;

        animController.UpdateMoveParams(localMove, moveSpeed / 10f, isMoved);
    }

    private void OnMove(Vector2 input)
    {
        if (!InputManager.Instance.IsInputEnabled) return;

        moveInput = input;
        unitController.Move(new Vector3(input.x, 0, input.y), moveSpeed);
    }

    private void OnAbleReload(int ammo)
    {
        isAbleReload = ammo < playerAttack.MaxAmmo;
    }

    private void OnReload()
    {
        if (!isAbleReload) return;           // 재장전 가능 여부 확인
        if (isOnReload) return;              // 재장전 중인지 확인
        isOnReload = true;

        // 재장전 애니메이션 길이에 맞춰 효과음 피치를 조정.
        float animLength = animController.OnReload();
        AudioClip reloadSfx = weaponManager.WeaponData.reloadSFX;
        float pitch = Mathf.Clamp(reloadSfx.length / animLength, 0.8f, 1.5f);
        GameManager.Instance.SoundMgr.PlaySfx(reloadSfx, followTarget: transform, pitch: pitch);

        playerAttack.TryReload();
    }

    private void OnReloadComplete(int ammo)
    {
        isOnReload = false;
        isAbleReload = false;
    }

    private void OnReloadFail()
    {
        isOnReload = false;
    }
}
