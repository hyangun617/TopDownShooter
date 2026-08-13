using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

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

    // 키 생성
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
    /// </summary>
    public async Task LoadLabelAsync<T>(string label) where T : UnityEngine.Object
    {
        var handlekey = MakeHandleKey<T>(label);
        if(handles.ContainsKey(handlekey)) return;

        var cacheKeys = new List<string>();
        labelToCacheKeys[handlekey] = cacheKeys;

        var handle = Addressables.LoadAssetsAsync<T>(label, asset =>
        {
            string cacheKey = $"{label}_{asset.name}";
            SetCache(cacheKey, asset);
            cacheKeys.Add(asset.name);
        });

        handles[handlekey] = handle;
        await handle.Task;
    }

    /// <summary>
    /// 캐싱된 리소스를 이름으로 조회, 없으면 null
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

    public void ReleaseLabel<T>(string label) where T : UnityEngine.Object
    {
        var handleKey = MakeHandleKey<T>(label);

        // 핸들 해제
        if(handles.TryGetValue(handleKey, out var handle))
        {
            Addressables.Release(handle);
            handles.Remove(handleKey);
        }

        // 캐시에 해당 항목들 제거
        if(labelToCacheKeys.TryGetValue(handleKey, out var cacheKey))
        {
            if(caches.TryGetValue(typeof(T), out var dict))
            {
                foreach(var key in cacheKey) dict.Remove(key);

                // 해당 타입의 캐시가 완전히 비었으면 타입 엔트리 자체도 제거
                if(dict.Count == 0) caches.Remove(typeof(T));
            }

            labelToCacheKeys.Remove(handleKey);
        }
    }

    public void RelaseAll()
    {
        foreach(var handle in handles.Values)   Addressables.Release(handle);

        handles.Clear();
        caches.Clear();
        labelToCacheKeys.Clear();
    }
    
}
