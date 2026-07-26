using UnityEngine;

// 카메라를 향해 항상 오브젝트가 회전함.
public class Billboard : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        // 카메라의 정면을 향해 회전.
        transform.forward = cam.transform.forward;
    }
}
