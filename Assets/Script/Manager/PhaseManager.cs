using UnityEngine;
using System.Collections;

// 게임의 전반적인 진행 방식을 다루는 매니저.

/// <summary>
/// [게임 시작 전] -> [게임 시작] -> [게임 중] -> [게임 종료 전] -> [게임 종료]
/// </summary>
public class PhaseManager : MonoBehaviour
{
    [SerializeField] EnemySpawner spawner;
    [SerializeField] PlayerManager playerMgr;
    [SerializeField] InfiniteBackground tileGenerator;

    // 게임 상태
    private bool isGameEnd = false;

    // 코루틴 핸들러
    private Coroutine gameFlowHandler;

    void Awake()
    {
        if(spawner == null) spawner = GetComponentInChildren<EnemySpawner>();
        if(playerMgr == null) playerMgr = GetComponentInChildren<PlayerManager>();
        if(tileGenerator == null) tileGenerator = GetComponentInChildren<InfiniteBackground>();
    }

    void Start()
    {
        Init();

        gameFlowHandler = StartCoroutine(GameFlow());
    }

    // 초기화
    private void Init()
    {
        Time.timeScale = 0f;            // 시간 정지
        spawner.PauseSpawner();         // 스포너 정지
    }

    private IEnumerator GameFlow()
    {
        PreGameStart();

        yield return new WaitForSecondsRealtime(5f);

        GameStart();

        yield return StartCoroutine(GameLoop());

        yield return StartCoroutine(PreEndGame());

        EndGame();
    }

    // 게임 시작 전
    private void PreGameStart()
    {
        
    }

    // 게임 시작
    private void GameStart()
    {
        Time.timeScale = 1f;            // 시간 정지 해제
        spawner.ResumeSpawner();        // 스포너 실행
    }

    // 게임 중
    private IEnumerator GameLoop()
    {
        Debug.Log("[PhaseManager] GameLoop entered");

        while(!isGameEnd)
        {
            yield return null;
        }

        Debug.Log("[PhaseManager] GameLoop ended");
    }

    // 게임 종료 전
    private IEnumerator PreEndGame()
    {
        yield return null;
    }

    // 게임 종료
    private void EndGame()
    {
        
    }
}
