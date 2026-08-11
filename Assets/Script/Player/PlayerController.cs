using UnityEngine;

[RequireComponent(typeof(PlayerAnimController), typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    // 플레이어 객체
    private Rigidbody rb;              
    private Vector2 moveInput;      
    private PlayerAnimController playerAnimController;
    private PlayerAttack playerAttack;

    [SerializeField] private bool isOnReload = false;

    private float moveSpeed;

    void OnEnable()
    {
        playerAttack.isAmmoZero += OnReload;
        playerAttack.OnReloadComplete += OnReloadComplete;
        playerAttack.OnReloadFailed += OnReloadComplete;
        InputManager.Instance.OnMove += OnMove;
        InputManager.Instance.OnPressed_R += OnReload;
    }

    void OnDisable()
    {
        playerAttack.isAmmoZero -= OnReload;
        playerAttack.OnReloadComplete -= OnReloadComplete;
        playerAttack.OnReloadFailed -= OnReloadComplete;
        InputManager.Instance.OnMove -= OnMove;
        InputManager.Instance.OnPressed_R -= OnReload;
    }

    private void Awake()
    {
        // GameObject에 등록된 컴포넌트를 받아옴.
        rb = GetComponent<Rigidbody>();
        playerAnimController = GetComponent<PlayerAnimController>();
        playerAttack = GetComponent<PlayerAttack>();
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

    private void OnReload()
    {
        if(isOnReload) return;
        SetOnReloadState(true);

        // 리로드 메서드
        playerAnimController.OnReload();      
        playerAttack.TryReload();
    }

    private void OnReloadComplete(int val) => isOnReload = false;
    private void OnReloadComplete() => isOnReload = false;

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

    private void SetOnReloadState(bool state)
    {
        if(isOnReload == state) return;

        isOnReload = state;
    }

}
