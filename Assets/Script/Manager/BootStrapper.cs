using UnityEngine;

public class BootStrapper : MonoBehaviour
{
    // 각 매니저들의 인스턴스와 초기화 시점을 보장함.

    [SerializeField] private GameManager gameManager;
    [SerializeField] private InputManager inputManager;

    private void Awake()
    {
        // 이미 GameManager가 존재한다면
        // BootStrapper가 다시 로드 되어 중복 초기화가 되는 것을 예방함.
        if(GameManager.Instance != null)
        {
            Destroy(gameObject);    // Bootstrapper 스스로를 파괴.
            return;
        }

        DontDestroyOnLoad(gameManager.gameObject);
        DontDestroyOnLoad(inputManager.gameObject);

        // 초기화 시점 강제.
        inputManager.RegisterAsInstance();
        gameManager.RegisterAsInstance();

        // 각 매니저 초기화 이벤트 구독
        inputManager.OnInputInitialized += () =>
        {
            Debug.Log("Input Manager Initialized");
        };
        inputManager.Init();

        gameManager.Init();
    }
}
