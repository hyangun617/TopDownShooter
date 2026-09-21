using UnityEngine;

// Rigidbody 기반 이동 / 회전. Player(입력)와 Enemy(AI) 모두 이 컴포넌트를 통해 움직인다.
[RequireComponent(typeof(Rigidbody))]
public class UnitController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 moveDirection;
    private float moveSpeed;

    public Vector3 Position => rb.position;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        StopMoving();
    }

    private void FixedUpdate()
    {
        if (moveDirection == Vector3.zero) return;

        rb.MovePosition(rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime));
    }

    // 방향 기반 이동. 회전은 하지 않음 (입력 기반 유닛용).
    public void Move(Vector3 direction, float speed)
    {
        moveDirection = direction;
        moveSpeed = speed;
    }

    // 목표 지점을 향해 회전하며 이동 (AI 유닛용). 수평 이동만 한다.
    public void MoveToward(Vector3 targetPosition, float speed)
    {
        Vector3 direction = targetPosition - rb.position;
        direction.y = 0f;
        direction.Normalize();

        Rotate(direction);
        Move(direction, speed);
    }

    public void StopMoving()
    {
        moveDirection = Vector3.zero;
    }

    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
            rb.rotation = Quaternion.LookRotation(direction);
    }

    // 월드 좌표의 한 지점을 바라봄. y 좌표는 무시한다.
    public void LookAt(Vector3 point)
    {
        Vector3 direction = point - rb.position;
        direction.y = 0f;
        Rotate(direction);
    }
}
