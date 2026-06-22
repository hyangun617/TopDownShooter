using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        }
        Debug.Log("Game Manager Init");        

        // 하위 매니저들 초기화.
        GetComponentInChildren<InputManager>().Init();
    }

    private void Update()
    {
        
    }

}
