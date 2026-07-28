using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Cysharp.Threading.Tasks;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public bool IsInputEnabled { get; set; } = true;                // 입력 활성화 여부    
    public bool IsUIMode { get; private set; } = false;
    
#region Init

    public event Action OnInputInitialized;                 // InputManager 초기화 완료 이벤트
    public event Action<bool> OnInputModeChanged;           // true = UI Mode

    // 참조 세팅
    private void Awake()
    {
        inputSystem = new InputSystem();
    }

    // 인스턴스 생성 메서드.
    public void RegisterAsInstance()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 초기화. 
    // GameManager.cs에서 호출됨.
    public void Init()
    {    
        cam = Camera.main;                                         // 메인 카메라 할당.
        groundLayer = LayerMask.GetMask("Ground");                  // 레이어 마스크 참조, 해당 이름으로 된 레이어가 지정된 순서를 읽어옴.
        OnInputInitialized?.Invoke();                              // InputManager 초기화 완료 이벤트 호출.
    }

    // 입력 모드를 변경하는 메서드
    public void SetUIMode(bool uiMode)
    {
        if(IsUIMode == uiMode) return;

        IsUIMode = uiMode;

        if(uiMode)
        {
            inputSystem.Player.Disable();
            inputSystem.UI.Enable();
            GameManager.Instance.ChangeState(GameState.Paused);
        }
        else
        {
            inputSystem.Player.Enable();
            inputSystem.UI.Disable();
            GameManager.Instance.ChangeState(GameState.Playing);
        }

        OnInputModeChanged?.Invoke(uiMode);
    }

    private void OnEnable()
    {
        inputSystem.Player.MovePosition.performed += onMouseMoved;      // 마우스 움직임
        inputSystem.Player.Fire.performed += OnFirePerformed;           // 마우스 클릭
        inputSystem.Player.Fire.canceled += OnFireCanceled;
        inputSystem.Player.Move.performed += OnPlayerMovement;          // 움직임
        inputSystem.Player.Move.canceled += OnPlayerMovement;
        inputSystem.Player.Reload.performed += OnPressedRKey;           // 재장전

        inputSystem.Player.Cancel.performed += OnMenuKeyPressed;
        inputSystem.UI.Cancel.performed += OnUICancelPressed;

        UIManager.Instance.OnUIStackChanged += HandleUIStackChanged;

        inputSystem.Player.Enable();
        SetUIMode(false);
    }

    private void OnDisable()
    {
        inputSystem.Player.MovePosition.performed -= onMouseMoved;              
        inputSystem.Player.Fire.performed -= OnFirePerformed;                   
        inputSystem.Player.Fire.canceled -= OnFireCanceled;
        inputSystem.Player.Move.performed -= OnPlayerMovement;
        inputSystem.Player.Move.canceled -= OnPlayerMovement;
        inputSystem.Player.Reload.performed -= OnPressedRKey;

        inputSystem.Player.Cancel.performed -= OnMenuKeyPressed;
        inputSystem.UI.Cancel.performed -= OnUICancelPressed;

        UIManager.Instance.OnUIStackChanged -= HandleUIStackChanged;
    }

#endregion

#region UI

    private void HandleUIStackChanged(bool hasOpenedView)
    {
        SetUIMode(hasOpenedView);
    }

    // Player Input Map에서 ESC -> 메뉴 화면 열기.
    private void OnMenuKeyPressed(InputAction.CallbackContext ctx)
    {
        UIManager.Instance.OpenAsync<MenuView>().Forget();
    }

    // UI Input Map에서 ESC -> UI 닫기.
    private void OnUICancelPressed(InputAction.CallbackContext ctx)
    {
        UIManager.Instance.CloseTop();
    }

#endregion

#region Mouse

    [SerializeField] private int groundLayer;                       // Physics.Raycast의 레이어 마스크를 위한 int 변수.

    private Camera cam;                                             // 카메라를 기준으로 마우스의 위치를 읽어오기 때문에 카메라 객체를 읽어와야함.

    private Vector2 mouseScreenPos;                                 // 실시간 마우스 위치.
    private InputSystem inputSystem;                                // 마우스 입력을 받아올 InputSystem
    public Vector3 mouseWorldPos { get; private set; }              // 레이캐스트를 통해 구한 마우스의 위치를 저장하는 프로퍼티

    private float fireHoldTime = 0f;                                // 입력을 길게 누른 시간.

    public event Action<FireEventArgs> OnFire;                      // 발사 입력값을 전달할 이벤트

    private void UpdateMouseWorldPosition()
    {
        // 마우스가 없다면, 함수 실행 안함.
        if (Pointer.current == null) return;

        // 방어적 코드.
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        // Raycast
        // 카메라에서 부터 마우스의 위치까지 직선(ray)을 그음.
        Ray ray = cam.ScreenPointToRay(mouseScreenPos);

        // 바닥에 닿은 지점의 포지션을 groundHit에 저장함.
        // Mathf.Infinity -> 무한수 -> ray는 바닥만을 감지하기에 성능에도 문제 없음.
        if (!Physics.Raycast(ray, out RaycastHit groundHit, Mathf.Infinity, groundLayer)) return;

        mouseWorldPos = groundHit.point;
    }

    // 마우스가 움직일 때 마다 해당 마우스의 위치를 갱신.
    private void onMouseMoved(InputAction.CallbackContext ctx)
    {
        mouseScreenPos = ctx.ReadValue<Vector2>();
    }

    // 마우스의 입력 시작
    private void OnFirePerformed(InputAction.CallbackContext ctx)
    {
        fireHoldTime = 0f;

        // performed 시점의 아날로그 값도 같이 넘김
        OnFire?.Invoke(new FireEventArgs(true, 0f, 1f));
    }

    // 마우스의 입력 종료
    private void OnFireCanceled(InputAction.CallbackContext ctx)
    {
        OnFire?.Invoke(new FireEventArgs(false, fireHoldTime));
        fireHoldTime = 0f;
    }

#endregion

#region Keys

    public event Action<Vector2> OnMove;                            // 이동 입력값을 전달할 이벤트
    public event Action OnPressed_R;                                // R 키를 누르면 전달할 이벤트

    // 이동 입력 처리
    private void OnPlayerMovement(InputAction.CallbackContext ctx)
    {
        OnMove?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnPressedRKey(InputAction.CallbackContext ctx)
    {
        OnPressed_R?.Invoke();
    }

#endregion

    private void Update()
    {
        // 입력 활성화 여부
        if (!IsInputEnabled || IsUIMode) return;

        UpdateMouseWorldPosition();

        // 마우스 홀드 여부
        if (inputSystem.Player.Fire.IsPressed())
            fireHoldTime += Time.deltaTime;
    }

    private void OnDestroy()
    {
        if(Instance == this) Instance = null;
    }
}
