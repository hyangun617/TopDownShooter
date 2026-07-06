using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 pendingDirection;
    private float pendingSpeed;

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
        pendingDirection = (targetPosition - transform.position).normalized;
        pendingDirection.y = 0f; // 수평 이동

        // 타겟을 향해 회전
        transform.rotation = Quaternion.LookRotation(pendingDirection);

        pendingSpeed = speed; 
    }

    public void StopMoving()
    {
        pendingDirection = Vector3.zero;
    }
}