using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class DataManager
{
    // 데이터 목록
    public EnemyTB meleeEnemyTB;
    public EnemyTB rangeEnemyTB;
    public GameObject bulletPrefab;
    private Dictionary<string, GameObject> combatAssetCache = new();
    
    // 데이터 초기화 완료 여부
    public bool IsDataInitialized { get; private set; } = false;

    // 초기화 완료 이벤트
    public event Action OnDataInitialized;

    // 해제를 위한 핸들 보관
    private List<AsyncOperationHandle> handles = new();

    public async UniTask Init()
    {
        // 초기화 여부 확인. 이미 초기화가 완료된 경우에는 다시 초기화하지 않음.
        if(IsDataInitialized) return;

        var enemyDataHandle = Addressables.LoadAssetsAsync<EnemyTB>("GameData", null);
        handles.Add(enemyDataHandle);
        var tables = await enemyDataHandle.ToUniTask();

        foreach(var tb in tables)
        {
            tb.Init();
            switch(tb.enemyType)
            {
                case EnemyType.Melee:
                    {
                        meleeEnemyTB = tb;
                        break;
                    }
                case EnemyType.Range:
                    {
                        rangeEnemyTB = tb;
                        break;
                    }
            }
        }

        var CombatHandles = Addressables.LoadAssetsAsync<GameObject>("CombatAssets", null);
        handles.Add(CombatHandles);
        var AllCombatAssets = await CombatHandles.ToUniTask();

        foreach(var prefab in AllCombatAssets)
        {
            combatAssetCache[prefab.name] = prefab;
        }

        bulletPrefab = combatAssetCache.GetValueOrDefault("Bullet");

        IsDataInitialized = true;
        OnDataInitialized?.Invoke();
    }

    public GameObject GetCombatAsset(string name) => combatAssetCache.TryGetValue(name, out var prefab) ? prefab : null;

    public void ReleaseAll()
    {
        foreach(var handle in handles)
        {
            if(handle.IsValid()) Addressables.Release(handle);
        }

        handles.Clear();
    }
    
}
