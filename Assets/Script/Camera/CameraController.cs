using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 플레이어의 오브젝트를 담을 변수
    [SerializeField] private GameObject player;

    // 카메라가 플레이어로 부터 떨어질 오프셋
    public Vector3 offset;

    // 카메라가 아래를 바라보는 정도.
    // Quaternion = 회전값을 담기 위한 변수.
    public Quaternion Rotation;

    private void Awake()
    {
        // player 할당
        // FindWithTag를 이용해 Player 태그를 단 게임 오브젝트를 찾음.
        player = GameObject.FindWithTag("Player");

        // 회전 초기값 지정.
        Rotation = Quaternion.Euler(60f, 0f, 0f);
        // 오프셋 초기값 지정
        offset = new Vector3(0f, 10f, -5f);        
    }

    private void LateUpdate()
    {
        // 플레이어의 위치로 카메라 이동
        transform.position = player.transform.position + offset;

        // 카메라 회전.
        transform.rotation = Rotation;
        
    }

}
