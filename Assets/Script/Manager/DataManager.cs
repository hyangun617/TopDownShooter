using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DataManager
{    
    // 데이터 초기화 완료 여부
    public bool IsDataInitialized { get; private set; } = false;

    // 초기화 완료 이벤트
    public event Action OnDataInitialized;

    // 핸들 캐싱
    private readonly Dictionary<string, AsyncOperationHandle> handles = new();
    private readonly Dictionary<Type, Dictionary<string, object>> caches = new();
    private readonly Dictionary<string, List<string>> labelToCacheKeys = new();

    // 핸들 키 생성 (타입 + 라벨 조합, Release 관리용)
    private string MakeHandleKey<T>(string label) => $"{typeof(T).Name}_{label}";

    public void Init()
    {
        // 초기화 여부 확인. 이미 초기화가 완료된 경우에는 다시 초기화하지 않음.
        if(IsDataInitialized) return;

        IsDataInitialized = true;
        OnDataInitialized?.Invoke();
    }
    
    /// <summary>
    /// 라벨에 묶인 모든 T 타입 리소스를 로드하고 캐싱한다.
    /// 이미 로드된 라벨이면 재로드하지 않는다.
    /// 캐시 키는 asset.name 그대로 사용 (네이밍 컨벤션으로 충돌 방지 필요).
    /// </summary>
    public async Task LoadLabelAsync<T>(string label) where T : UnityEngine.Object
    {
        var handlekey = MakeHandleKey<T>(label);
        if(handles.ContainsKey(handlekey)) return;

        var cacheKeys = new List<string>();
        labelToCacheKeys[handlekey] = cacheKeys;

        var handle = Addressables.LoadAssetsAsync<T>(label, asset =>
        {
            // Debug.Log($"[DataManager] 캐시 저장: 타입={typeof(T).Name}, 키='{asset.name}'"); // 임시 로그
            SetCache(asset.name, asset);
            cacheKeys.Add(asset.name); // 캐시에 저장한 키와 동일한 값을 추적 리스트에 기록
        });

        handles[handlekey] = handle;
        await handle.Task;

        // 디버그 로그
        Debug.Log($"[DataManager] '{label}' 라벨로 {typeof(T).Name} 타입 {cacheKeys.Count}개 로드 완료");
        if (cacheKeys.Count == 0)
        Debug.LogWarning($"[DataManager] '{label}' 라벨에 매칭되는 {typeof(T).Name} 에셋이 Addressables에 하나도 없습니다. Label 설정을 확인하세요.");
    }

    /// <summary>
    /// 캐싱된 리소스를 이름으로 조회, 없으면 null.
    /// 주의: 같은 T 타입 내에서 이름이 겹치면 나중에 로드된 것으로 덮어써짐.
    /// (예: Enemy 라벨의 "Panel"과 UI 라벨의 "Panel"이 GameObject 타입으로 동시에 존재하면 충돌)
    /// </summary>
    public T Get<T>(string assetName) where T : UnityEngine.Object
    {
        if(caches.TryGetValue(typeof(T), out var dict) &&
            dict.TryGetValue(assetName, out var obj))
        {
            return obj as T;
        }

        Debug.LogWarning($"[DataManager] {typeof(T).Name} 타입의 '{assetName}'리소스가 캐시에 존재하지 않습니다.");
        return null;
    }

    /// <summary>
    /// 특정 타입 전체를 IReadOnlyDictionary로 조회
    /// </summary>
    public IReadOnlyDictionary<string, T> GetAll<T>() where T : UnityEngine.Object
    {
        if(!caches.TryGetValue(typeof(T), out var dict)) return new Dictionary<string, T>();
        
        var result = new Dictionary<string, T>();
        foreach(var kv in dict)
        {
            result[kv.Key] = kv.Value as T;
        }

        return result;
    }

    private void SetCache<T>(string key, T value) where T : UnityEngine.Object
    {
        if(!caches.TryGetValue(typeof(T), out var dict))
        {
            dict = new Dictionary<string, object>();
            caches[typeof(T)] = dict;
        }
        dict[key] = value;
    }

    /// <summary>
    /// 특정 라벨로 로드된 리소스만 정확히 해제 (핸들 + 캐시 항목 모두)
    /// </summary>
    public void ReleaseLabel<T>(string label) where T : UnityEngine.Object
    {
        var handleKey = MakeHandleKey<T>(label);

        // 핸들 해제
        if(handles.TryGetValue(handleKey, out var handle))
        {
            Addressables.Release(handle);
            handles.Remove(handleKey);
        }

        // 캐시에 해당 항목들 제거 (이제 저장 키와 동일한 값을 지우므로 정상 동작)
        if(labelToCacheKeys.TryGetValue(handleKey, out var cacheKeys))
        {
            if(caches.TryGetValue(typeof(T), out var dict))
            {
                foreach(var key in cacheKeys) dict.Remove(key);

                // 해당 타입의 캐시가 완전히 비었으면 타입 엔트리 자체도 제거
                if(dict.Count == 0) caches.Remove(typeof(T));
            }

            labelToCacheKeys.Remove(handleKey);
        }
    }

    public void RelaseAll()
    {
        foreach(var handle in handles.Values)   
            Addressables.Release(handle);

        handles.Clear();
        caches.Clear();
        labelToCacheKeys.Clear();
    }
    
}