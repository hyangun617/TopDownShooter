using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 pendingDirection;
    private float pendingSpeed;
    public Vector3 Position => rb.position;

    public void Initialize()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(pendingDirection != Vector3.zero)
        {
            rb.MovePosition(rb.position + pendingDirection * pendingSpeed * Time.fixedDeltaTime);
        }
    }

    public void MoveToward(Vector3 targetPosition, float speed)
    {
        pendingDirection = (targetPosition - rb.position).normalized;
        pendingDirection.y = 0f; // 수평 이동

        // 타겟을 향해 회전
        if(pendingDirection != Vector3.zero)
            rb.rotation = Quaternion.LookRotation(pendingDirection);

        pendingSpeed = speed; 
    }

    public void StopMoving()
    {
        pendingDirection = Vector3.zero;
    }

    public void Rotate(Vector3 direction)
    {   
        if(direction != Vector3.zero)
        rb.rotation = Quaternion.LookRotation(direction);
    }
}