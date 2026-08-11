using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Transform canvasRoot;

    // 라벨로 찾은 리소스 위치. Unload 이후 재로드할 때 사용.
    private Dictionary<Type, IResourceLocation> viewLocations = new();

    private Dictionary<Type, UIView> loadedViews = new();
    private Dictionary<Type, AsyncOperationHandle<GameObject>> loadedHandles = new();
    private Dictionary<Type, UniTask> loadingTask = new();

    private Stack<UIView> openedStack = new();

    public event Action<bool> OnUIStackChanged;     // true = 하나 이상의 UI가 열려 있음.

    public event Action PreloadCompleted;

    public void Init()
    {

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(canvasRoot == null)
            Debug.LogWarning($"[UIManager] {scene.name}에 UICanvasRoot가 없습니다");
    }

    public void RegisterAsInstance()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // RootCanvas를 등록하는 메서드
    public void SetCanvasRoot(Transform root)
    {
        canvasRoot = root;

        foreach(var view in loadedViews.Values)
        {
            view.transform.SetParent(canvasRoot, false);
        }

        Debug.Log("CanvasRoot 할당 완료");
    }
    
    // UI Prefab 프리로드 메서드
    public async UniTask PreloadAllAsync(string uiLabel)
    {
        var locHandle = Addressables.LoadResourceLocationsAsync(uiLabel, typeof(GameObject));
        var locations = await locHandle.ToUniTask();

        if(locations == null || locations.Count == 0)
        {
            Debug.LogError($"'{uiLabel}' 라벨에 등록된 UI 에셋이 없습니다.");
            Addressables.Release(locHandle);
            return;
        }

        var tasks = new List<UniTask>();
        foreach(var location in locations)
            tasks.Add(LoadAndCacheOnly(location));

        await UniTask.WhenAll(tasks);

        // 위치 조회용 핸들은 여기서 해제 (실제 에셋 핸들과는 별개)
        Addressables.Release(locHandle);

        PreloadCompleted?.Invoke();
    }

    private async UniTask LoadAndCacheOnly(IResourceLocation location)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(location);
        await handle.ToUniTask();

        if(handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"UI 프리로드 실패 : {location.PrimaryKey}");
            return;
        }

        var view = Instantiate(handle.Result, canvasRoot).GetComponent<UIView>();
        if(view == null)
        {
            Debug.LogError($"{location.PrimaryKey} 프리팹에 UIView 컴포넌트가 없습니다.");
            Addressables.Release(handle);
            return;
        }
        view.Close();

        var type = view.GetType();          // 실제 컴포넌트 타입을 그대로 키로 사용
        loadedViews[type] = view;
        loadedHandles[type] = handle;
        viewLocations[type] = location;
    }

    public async UniTask<T> OpenAsync<T>() where T : UIView
    {
        var type = typeof(T);

        // 이미 캐싱되어 존재한다면,
        if(loadedViews.TryGetValue(type, out var existing))
        {
            if(existing.IsModal && !openedStack.Contains(existing))
                PushToStack(existing);
            
            existing.Open();            
            return existing as T;
        }

        // 로딩 중인 상황이라면,
        if(loadingTask.TryGetValue(type, out var ongoing))
        {
            // 로딩이 될 때 까지 기다림
            await ongoing;

            // 로딩 완료 후 
            // 캐싱 된 dictionary에서 type에 해당하는 값을 찾아 실행.
            loadedViews.TryGetValue(type, out var loaded);

            if(loaded != null)
            {
                if(loaded.IsModal && !openedStack.Contains(loaded))
                    PushToStack(loaded);

                loaded.Open();
            }

            return loaded as T;
        }

        // Unload 이후 재호출 상황.
        if(viewLocations.TryGetValue(type, out var location))
        {
            var task = LoadAndCacheOnly(location);
            loadingTask[type] = task;
            await task;
            loadingTask.Remove(type);

            loadedViews.TryGetValue(type, out var reloaded);
            if(reloaded != null)
            {
                if(reloaded.IsModal && !openedStack.Contains(reloaded))
                    PushToStack(reloaded);

                reloaded.Open();
            }

            return reloaded as T;
        }

        // 이 아래는 메모리에 해당 UI가 없는 상황.
        Debug.LogError($"{type.Name}은 등록되지 않았습니다. Addressables Label을 확인해주세요");
        return null;
    }

    private void PushToStack(UIView view)
    {
        openedStack.Push(view);
        OnUIStackChanged?.Invoke(true);
    }

    private UIView PopFromStack()
    {
        var view = openedStack.Pop();
        OnUIStackChanged?.Invoke(openedStack.Count > 0);

        return view;
    }

    // UI 닫기
    public void Close<T>() where T : UIView
    {
        // 캐싱된 view가 아니라면 무시.
        if(!loadedViews.TryGetValue(typeof(T), out var view)) return;

        if(view.IsModal && openedStack.Contains(view))
        {
            // 스택 안에 있을 때만 제거.
            var temp = new Stack<UIView>();
            while(openedStack.Count > 0)
            {
                var top = openedStack.Pop();
                if(top == view)
                    break;

                temp.Push(top);
            }

            while(temp.Count > 0)
                openedStack.Push(temp.Pop());

            OnUIStackChanged?.Invoke(openedStack.Count > 0);
        }

        view.Close();
    }

    // 가장 마지막 UI 닫기.
    public void CloseTop()
    {
        if (openedStack.Count == 0) return;
        PopFromStack().Close();
    }

    // UI 등록 해제.
    public void Unload<T>() where T : UIView
    {
        if(loadedViews.TryGetValue(typeof(T), out var view))
        {
            Destroy(view.gameObject);
            loadedViews.Remove(typeof(T));
        }
        if(loadedHandles.TryGetValue(typeof(T), out var handle))
        {
            Addressables.Release(handle);
            loadedHandles.Remove(typeof(T));
        }
    }

    // 모든 UI 등록 해제.
    // keepTypes = 유지할 타입 목록.
    public void UnloadAllExcept(HashSet<Type> keepTypes)
    {
        var toRemove = new List<Type>();

        foreach(var type in loadedViews.Keys)
        {
            if(!keepTypes.Contains(type))
                toRemove.Add(type);
        }

        foreach(var type in toRemove)
        {
            Destroy(loadedViews[type].gameObject);
            loadedViews.Remove(type);
            Addressables.Release(loadedHandles[type]);
            loadedHandles.Remove(type);
        }
    }
}
