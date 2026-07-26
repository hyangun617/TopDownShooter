using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 순수 C# 매니저
    public DataManager DataMgr { get; private set; }
    public SoundManager SoundMgr { get; private set; }
    public PoolManager PoolMgr { get; private set; }
    public SettingManager SettingMgr { get; private set; }

    // 게임 상태
    public GameState CurrentState { get; private set; }

    // 상태 변화시 호출하는 이벤트
    public event Action<GameState> OnGameStateChanged;

    // Game State Setting
    [Header("Game Stat")]
    [SerializeField] private float endTime = 600f;
    [SerializeField] private int score = 0;
    public float EndTime => endTime;
    public int Score => score;

    public void SetScore(int value) => score = value;

    public async UniTaskVoid Init()
    {
        // 게임 매니저 초기화.

        // 하위 매니저 생성 및 초기화.
        DataMgr = new DataManager();
        SoundMgr = new SoundManager(this.transform);
        PoolMgr = gameObject.AddComponent<PoolManager>();      // MonoBehavior를 상속 받았기에 new 사용 불가.

        // 각 순수 C# 매니저 초기화
        await DataMgr.Init();

        Debug.Log("All Managers Ready");
    }

    // 인스턴스 생성 메서드
    public void RegisterAsInstance()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this; 
    }

    // 상태 변환 메서드
    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        HandleStateEnter(newState);
        OnGameStateChanged?.Invoke(newState);
    }

    // 상태 변환자 핸들러
    private void HandleStateEnter(GameState state)
    {
        switch(state)
        {
            case GameState.MainMenu:
                {
                    // 메인 메뉴
                    Time.timeScale = 1f;
                    break;
                }
            case GameState.Playing:
                {
                    // 정상 속도로 게임 진행.
                    Debug.Log("Game on Playing!");
                    Time.timeScale = 1f;
                    break;
                }
            case GameState.Paused:
                {
                    // 게임 일시 정지.
                    Debug.Log("Game Paused!");
                    Time.timeScale = 0f;
                    break;
                }
            case GameState.GameOver:
                {
                    Time.timeScale = 0f;
                    Debug.Log("Game Over!");

                    // 게임 일시 정지 후, 각종 결과 데이터 확정.

                    break;
                }
            case GameState.Result:
                {
                    Time.timeScale = 0f;
                    Debug.Log("Game Clear!");

                    break;
                }
        }
    }
}
