using System;

public class ScoreManager
{
    // 초기화 완료 이벤트 및 불리언
    public bool IsScoreManagerInitialized {get; private set;} = false;
    public event Action OnScoreManagerInitialized;

    // 점수
    private int _score = 0;
    public int Score => _score;

    public void Init()
    {
        
    }
}