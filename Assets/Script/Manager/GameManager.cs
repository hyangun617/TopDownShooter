using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 순수 C# 매니저
    public DataManager Data { get; private set; }
    public ScoreManager ScoreMgr { get; private set; }
    public TimeManager TimeMgr { get; private set; }
    public SoundManager Sound { get; private set; }

    // 게임 상태
    public GameState CurrentState { get; private set; }

    // 각종 파라미터
    [SerializeField] private float bgm_Volume = 1f;
    [SerializeField] private float sfx_Volume = 1f;

    // 상태 변화시 호출하는 이벤트
    public event Action<GameState> OnGameStateChanged;

    public void Init()
    {
        // 게임 매니저 초기화.
        Debug.Log("Game Manager Initialized"); 

        // 하위 매니저 생성 및 초기화.
        Data = new DataManager();
        ScoreMgr = new ScoreManager();
        TimeMgr = new TimeManager();
        Sound = new SoundManager(this.transform);
        Sound.BGMVolume = bgm_Volume;
        Sound.SFXVolume = sfx_Volume;

        Data.OnDataInitialized += () => { Debug.Log("Data Manager Initialized"); };

        // 각 순수 C# 매니저 초기화
        Data.Init();
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
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 값 범위 검증
        bgm_Volume = Mathf.Clamp01(bgm_Volume);
        sfx_Volume = Mathf.Clamp01(sfx_Volume);
    }
#endif
}
