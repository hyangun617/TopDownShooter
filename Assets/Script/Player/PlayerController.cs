using UnityEngine;
using UnityEngine.InputSystem; // 인풋 시스템을 사용하기 위해 네임 스페이스를 읽어옴.

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;              
    private Vector2 moveInput;      

    // 이동 속도
    [SerializeField] private float moveSpeed = 10f;
    

    private void Awake()
    {
        // GameObject에 등록된 Rigidbody 컴포넌트를 받아옴.
        rb = GetComponent<Rigidbody>();
    }

    // 해당 플레이어가 활성화 되었을 때.
    private void OnEnable()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputManager.Instance.OnMove += onMove;
    }

    // Update is called once per frame
    void Update()
    {
        // 입력이 활성화 되어 있을 때만 회전.
        if (InputManager.Instance != null && InputManager.Instance.IsInputEnabled)
        {
            LookAtMouse();
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

    // 플레이어 오브젝트가 비활성화 일 경우 입력도 비활성화.
    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnMove -= onMove;
    }

    void onMove(Vector2 input)
    {
        if (InputManager.Instance.IsInputEnabled)
        {
            moveInput = input;
        }        
    }

}
