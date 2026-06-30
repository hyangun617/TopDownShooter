using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 순수 C# 매니저
    public DataManager Data { get; private set; } = new DataManager();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 씬 로드시 이전 객체가 남아있다면 새로운 객체는 제거.
            Destroy(gameObject);
            return;
        }
        Debug.Log("Game Manager Init");        

        // 각 매니저 초기화 이벤트 구독
        InputManager.Instance.OnInputInitialized += () =>
        {
            Debug.Log("Input Manager Initialized");
        };

        Data.OnDataInitialized += () =>
        {
            Debug.Log("Data Manager Initialized");
        };

        // 각 순수 C# 매니저 초기화
        Data.Init();
    }
}
