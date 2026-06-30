using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System;

public class DataManager
{
    // 데이터 목록
    public EnemyTB enemyTB;
    // 데이터 초기화 완료 여부
    public bool _isDataInitialized { get; private set; } = false;

    // 초기화 완료 이벤트
    public event Action OnDataInitialized;

    public void Init()
    {
        // 초기화 여부 확인. 이미 초기화가 완료된 경우에는 다시 초기화하지 않음.
        if(_isDataInitialized)
        {
            Debug.Log("DataManager is already initialized.");
            return;
        }

        // Addressable을 이용한 콜백 방식의 비동기 데이터 로드
        Addressables.LoadAssetAsync<EnemyTB>("Assets/Prefebs/Enemy/EnemyTB.asset").Completed += Handle =>
        {
            if(Handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 비동기 에셋 로드에 성공했다면 실행
                enemyTB = Handle.Result;
                enemyTB.Init();
                Debug.Log("EnemyTB Load Success");

                // 완료된 경우 GameManager에 알림
                _isDataInitialized = true;
                OnDataInitialized?.Invoke();
            }
            else
            {
                Debug.LogError("EnemyTb Load Failed");
            }
        };
    }
}
