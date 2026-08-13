using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public enum LoadState
{
    SceneLoad,
    LabelLoad,
    PrefabLoad,
    ObjectLoad,
    UILoad,
    ReleaseData
};

public class LoadManager
{
    // Label List Cache
    private readonly Dictionary<string, ScenePrefabs> PrefabList = new();
    private readonly Dictionary<string, AsyncOperationHandle> handles = new();
    // 각 타입에 맞는 로드 함수 매핑
    private readonly Dictionary<PrefabType, Func<string, Task>> loaders;

    // 로드 현황을 알려주는 이벤트
    // 로드 대상, 상태, 진행률
    public event Action<string, LoadState, float> broadCastLoadState;

    public LoadManager()
    {
        loaders = new Dictionary<PrefabType, Func<string, Task>>
        {
            {PrefabType.AudioClip, label => GameManager.Instance.DataMgr.LoadLabelAsync<AudioClip>(label)},
            {PrefabType.GameObject, label => GameManager.Instance.DataMgr.LoadLabelAsync<GameObject>(label)},
            {PrefabType.Material, label => GameManager.Instance.DataMgr.LoadLabelAsync<Material>(label)},
        };
    }
    

#region Caching Scene Prefab Label List

    public async UniTask<ScenePrefabs> LoadPrefabList(string sceneName)
    {
        broadCastLoadState?.Invoke(sceneName, LoadState.LabelLoad, 0f);
        
        if(PrefabList.TryGetValue(sceneName, out var cached))
        {
            Debug.LogWarning($"[LoadManager] {sceneName}의 프리팹은 이미 캐싱되어 있습니다.");
            return cached;
        }

        var handle = Addressables.LoadAssetAsync<ScenePrefabs>(sceneName);

        await handle.Task;

        // 실패 검사
        if(handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            Debug.LogError($"[LoadManager] '{sceneName}'에 대한 ScenePrefab 로드 실패");
            return null;
        }

        PrefabList[sceneName] = handle.Result;
        handles[sceneName] = handle;
        return handle.Result;
    }

#endregion

#region Scene Load
    private string loadSceneName = "";

    public string GetLoadSceneName() => loadSceneName;

    public event Action OnSceneLoadCompleted;

    public async UniTask SceneLoad(string sceneName)
    {
        loadSceneName = sceneName;
        broadCastLoadState?.Invoke(sceneName, LoadState.ReleaseData, 0f);

        // 씬 전환 시, 이전 씬의 팁제 데이터 메모리에서 해제.
        GameManager.Instance.DataMgr.RelaseAll();
        UIManager.Instance.DetachFromCanvas();

        broadCastLoadState?.Invoke(sceneName, LoadState.SceneLoad, 0f);

        await SceneManager.LoadSceneAsync("Load_Scene");

        // Label 캐시에서 sceneName의 프리팹 리스트가 존재하는지 확인
        if(!PrefabList.TryGetValue(sceneName, out var list))
        {
            // 존재하지 않는 경우 로드.
            list = await LoadPrefabList(sceneName);
        }

        if(list == null)
        {
            Debug.LogError($"[LoadManager] 씬 : {sceneName} 로드 실패, ScenePrefabs 파일이 존재하지 않습니다.");
            broadCastLoadState?.Invoke(sceneName, LoadState.ReleaseData, 0f);
            await SceneManager.LoadSceneAsync("Title_Scene");
            return;
        }

        // 씬 로드
        var sceneHandle = SceneManager.LoadSceneAsync(sceneName);
        sceneHandle.allowSceneActivation = false;

        await UniTask.WhenAll(
            TrackSceneLoadProgress(sceneHandle),
            LoadSceneResourcesAsync(list, sceneHandle)
        );

        // 모든 준비가 끝났으니 씬 활성화
        sceneHandle.allowSceneActivation = true;
        OnSceneLoadCompleted?.Invoke();
    }

    private async UniTask TrackSceneLoadProgress(AsyncOperation sceneHandle)
    {
        while(sceneHandle.progress < 0.9f)
        {
            float normalized = sceneHandle.progress / 0.9f;
            broadCastLoadState?.Invoke(loadSceneName, LoadState.SceneLoad, normalized * 0.5f);
            await UniTask.Yield();
        }
    }

    private async UniTask LoadSceneResourcesAsync(ScenePrefabs scenePrefabs, AsyncOperation sceneHandle)
    {
        int total = scenePrefabs.List.Count;
        int completed = 0;

        foreach(var prefabLabel in scenePrefabs.List)
        {
            if(loaders.TryGetValue(prefabLabel.Type, out var loader))
            {
                broadCastLoadState?.Invoke(prefabLabel.Label, LoadState.PrefabLoad, 0.5f + 0.5f * (float)completed / total);
                await loader(prefabLabel.Label);
                completed++;
            }
            else
            {
                Debug.LogError($"[LoadManager] 등록되지 않은 PrefabType: {prefabLabel.Type}");
            }
        }

        broadCastLoadState?.Invoke("", LoadState.PrefabLoad, 1f);
    }
#endregion
}