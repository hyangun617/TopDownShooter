using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 순수 C# 매니저
    public DataManager Data { get; private set; } = new DataManager();
    public ScoreManager ScoreMgr { get; private set; } = new ScoreManager();
    public TimeManager timeMgr { get; private set; } = new TimeManager();

    public GameState CurrentState { get; private set; }

    // 상태 변화시 호출하는 이벤트
    public event Action<GameState> OnGameStateChanged;

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
        }
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
