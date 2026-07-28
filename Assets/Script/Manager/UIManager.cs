using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private UIPreloadTable table;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PreloadAllAsync(table).Forget();
    }

#region HUD
    [SerializeField] private HUDController hudController;        // 기본으로 씬에 배치된 HUD;

    // 중재자.
    public void ReloadBind(PlayerAttack playerAttack) => hudController.ReloadBind(playerAttack);
#endregion

#region UIView

    // 가장 모든 UI의 상위 오브젝트.
    [SerializeField] private Transform canvasRoot;

    // Table의 Reference 들을 저장하는 컨테이너
    private Dictionary<Type, AssetReferenceGameObject> viewRefs = new();

    // 로드된 오브젝트 캐싱 컨테이너
    private Dictionary<Type, UIView> loadedViews = new();

    // 프리로드 된 핸들 저장 컨테이너
    private Dictionary<Type, AsyncOperationHandle<GameObject>> loadedHandles = new();
    // 현재 로드 중인 Task들을 저장하는 컨테이너
    private Dictionary<Type, UniTask> loadingTask = new();

    // Open() 된 순서를 저장하는 Stack 컨테이너
    private Stack<UIView> openedStack = new();

    public event Action<bool> OnUIStackChanged;     // true = 하나 이상의 UI가 열려 있음.

    // Enum <-> Type 매핑
    private static Type ResolveType(UIViewID id) => id switch
    {
        UIViewID.Menu => typeof(MenuView),
        UIViewID.Settings => typeof(SettingView),

        _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
    };

    
    // UI Prefab 프리로드 메서드
    public async UniTask PreloadAllAsync(UIPreloadTable table)
    {
        if(table == null)
        {
            Debug.LogError("UIPreloadTable이 존재하지 않습니다.");
            return;
        }

        foreach(var entry in table.entries)
        {
            var type = ResolveType(entry.viewID);
            viewRefs[type] = entry.prefabRef;

            var task = LoadAndCacheOnly(type, entry.prefabRef);
            loadingTask[type] = task;
            await task;
            loadingTask.Remove(type);
        }
    }

    private async UniTask LoadAndCacheOnly(Type type, AssetReferenceGameObject prefabRef)
    {
        var handle = prefabRef.LoadAssetAsync<GameObject>();
        await handle.ToUniTask();

        // 로드에 실패 했으면
        if(handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"UI 프리로드 실패 : {type.Name}");
            return;
        }

        var view = Instantiate(handle.Result, canvasRoot).GetComponent<UIView>();
        view.Close();

        loadedViews[type] = view;
        loadedHandles[type] = handle;
    }

    public async UniTask<T> OpenAsync<T>() where T : UIView
    {
        var type = typeof(T);

        // 이미 캐싱되어 존재한다면,
        if(loadedViews.TryGetValue(type, out var existing))
        {
            if(!openedStack.Contains(existing))
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
                if(!openedStack.Contains(loaded))
                    PushToStack(loaded);

                loaded.Open();
            }

            return loaded as T;
        }

        // Unload 이후 재호출 상황.
        if(viewRefs.TryGetValue(type, out var prefabRef))
        {
            var task = LoadAndCacheOnly(type, prefabRef);
            loadingTask[type] = task;
            await task;
            loadingTask.Remove(type);

            loadedViews.TryGetValue(type, out var reloaded);
            if(reloaded != null)
            {
                reloaded.Open();
                PushToStack(reloaded);
            }

            return reloaded as T;
        }

        // 이 아래는 PreloadTable에 없는 상황.
        Debug.LogError($"{type.Name}은 등록되지 않았습니다. UIPreloadTable SO을 확인해주세요");
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

        if(openedStack.Count > 0 && openedStack.Peek() == view)
        {
            view.Close();
            PopFromStack();
        }            
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

#endregion
}
