using UnityEngine;
using UnityEngine.InputSystem; // 인풋 시스템을 사용하기 위해 네임 스페이스를 읽어옴.

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;               // 움직임을 위해 물리연산자를 담을 변수.
    private Vector2 moveInput;       // 이동 방향.

    private InputSystem inputSystem;

    // 이동 속도
    public float moveSpeed;

    private void Awake()
    {
        // GameObject에 등록된 Rigidbody 컴포넌트를 받아옴.
        rb = GetComponent<Rigidbody>();
        inputSystem = new InputSystem();

        // 기본 이동 속도.
        moveSpeed = 10f;
    }

    // 해당 플레이어가 활성화 되었을 때.
    private void OnEnable()
    {
        inputSystem.Player.Enable();
        inputSystem.Player.Move.performed += onMove;
        inputSystem.Player.Move.canceled += onMove;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // 입력 값에 따라 단위 벡터를 받아옴.
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);        
    }

    // 플레이어 오브젝트가 비활성화 일 경우 입력도 비활성화.
    private void OnDisable()
    {
        inputSystem.Player.Move.performed -= onMove;
        inputSystem.Player.Move.canceled -= onMove;
        inputSystem.Player.Disable();
    }

    void onMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
}
