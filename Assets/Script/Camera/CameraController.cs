using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 플레이어의 오브젝트를 담을 변수
    [SerializeField] private Transform player;

    // 카메라가 플레이어로 부터 떨어질 오프셋
    public Vector3 offset;

    // 카메라가 아래를 바라보는 정도.
    // Quaternion = 회전값을 담기 위한 변수.
    public Quaternion Rotation;

    private void Awake()
    {
        // 회전 초기값 지정.
        Rotation = Quaternion.Euler(90f, 0f, 0f);
        // 오프셋 초기값 지정
        offset = new Vector3(0f, 20f, 0f);        
    }

    void OnEnable()
    {
        PlayerManager.OnPlayerSpawned += SetupPlayer;
    }

    void OnDisable()
    {
        PlayerManager.OnPlayerSpawned -= SetupPlayer;
    }

    private void SetupPlayer(Transform player)
    {
        this.player = player;
    }

    private void LateUpdate()
    {
        if(player == null) return;

        // 플레이어의 위치로 카메라 이동
        transform.position = player.position + offset;

        // 카메라 회전.
        transform.rotation = Rotation;        
    }
}
