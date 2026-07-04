using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System;

public class DataManager
{
    // 데이터 목록
    public EnemyTB meleeEnemyTB;
    public EnemyTB rangeEnemyTB;
    
    // 데이터 초기화 완료 여부
    public bool _isDataInitialized { get; private set; } = false;

    // 초기화 완료 이벤트
    public event Action OnDataInitialized;

    // 로드 완료 카운트
    private int loadedCount = 0;
    private const int TotalLoadCount = 2;       // 로드해야할 총 테이블 개수.

    public void Init()
    {
        // 초기화 여부 확인. 이미 초기화가 완료된 경우에는 다시 초기화하지 않음.
        if(_isDataInitialized)
        {
            Debug.Log("DataManager is already initialized.");
            return;
        }

        // Addressable을 이용한 콜백 방식의 비동기 데이터 로드
        Addressables.LoadAssetAsync<EnemyTB>("Assets/Prefebs/Enemy/Melee_Enemy_TB.asset").Completed += Handle =>
        {
            if(Handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 비동기 에셋 로드에 성공했다면 실행
                meleeEnemyTB = Handle.Result;
                meleeEnemyTB.Init();
                OnSingleTableLoaded();      // 로드 완료시 카운트 증가.

                Debug.Log("Melee EnemyTB Load Success");
            }
            else
            {
                Debug.LogError("Melee EnemyTB Load Failed");
            }
        };

        Addressables.LoadAssetAsync<EnemyTB>("Assets/Prefebs/Enemy/Range_Enemy_TB.asset").Completed += Handle =>
        {
            if(Handle.Status == AsyncOperationStatus.Succeeded)
            {
                rangeEnemyTB = Handle.Result;
                rangeEnemyTB.Init();
                OnSingleTableLoaded();      // 로드 완료시 카운트 증가.
                
                Debug.Log("Range EnemyTB Load Success");
            }
            else
            {
                Debug.LogError("Range EnemyTB Load Failed");
            }
        };    
    }

    // 로드 하나가 끝날 때마다 호출되어, "다 끝났는지" 판단하는 지점
    private void OnSingleTableLoaded()
    {
        loadedCount++;

        if (loadedCount >= TotalLoadCount)
        {
            // 완료된 경우 GameManager에 알림
            _isDataInitialized = true;
            OnDataInitialized?.Invoke();
        }
    }
}
