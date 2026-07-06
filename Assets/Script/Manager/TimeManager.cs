using System;
using UnityEngine;

public class TimeManager
{
    // 초기화 완료 불리언 및 이벤트
    public bool IsTimeManagerIntialized = false;
    public event Action OnTimeManagerInitialized;

    private Time timer;

    public void Init()
    {
        
    }
}