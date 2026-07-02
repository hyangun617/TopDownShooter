using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 순수 C# 매니저
    public DataManager Data { get; private set; } = new DataManager();

    private void Awake()
    {
        
    }

    // 인스턴스 생성 메서드
    public void RegisterAsInstance()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Debug.Log("Game Manager Instance Created"); 
    }

    public void Init()
    {
        // 게임 매니저 초기화.
        Debug.Log("Game Manager Init"); 

        Data.OnDataInitialized += () =>
        {
            Debug.Log("Data Manager Initialized");
        };

        // 각 순수 C# 매니저 초기화
        Data.Init();
    }
}
