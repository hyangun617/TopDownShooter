using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 worldOffset;
    [SerializeField] private Vector3 fixedEularRotation;

    [SerializeField] private bool usePosition = true;
    [SerializeField] private bool useRoation = true;

    private void LateUpdate()
    {
        // 위치 : 캐릭터의 회전과 무관하게, 월드 기준 오프셋만 더함.
        if(usePosition)
            transform.position = target.position + worldOffset;

        // 회전 : 캐릭터의 회전을 상속받지 않고, 항상 고정값으로 덮어씀.
        if(useRoation)
            transform.rotation = Quaternion.Euler(fixedEularRotation);
    }
}
