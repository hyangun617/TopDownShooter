using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

#region HUD
    [SerializeField] private HUDController hudController;        // 기본으로 씬에 배치된 HUD;

    // 중재자.
    public void ReloadBind(PlayerAttack playerAttack) => hudController.ReloadBind(playerAttack);
#endregion

#region UIView

    private Dictionary<Type, UIView> openedViews = new();       // UIView를 상속한 UI 컨테이너
    private Dictionary<Type, AsyncOperationHandle<GameObject>> loadedHandles = new();
    [SerializeField] private Transform canvasRoot;

    public async UniTask<T> OpenAsync<T>() where T : UIView
    {
        // 딕셔너리에서 캐싱된 값을 찾음
        if(openedViews.TryGetValue(typeof(T), out var existing))
        {
            existing.Open();
            return existing as T;
        }

        // 캐싱된 값이 없다면 새로 생성.
        // Address 값으로 메모리에 할당함.
        var handle = Addressables.LoadAssetAsync<GameObject>($"UI/{typeof(T).Name}");
        await handle.ToUniTask();

        if(handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"UI 로드 실패 : {typeof(T).Name}");
            return null;
        }

        var view = Instantiate(handle.Result, canvasRoot).GetComponent<T>();
        view.Open();

        openedViews[typeof(T)] = view;
        loadedHandles[typeof(T)] = handle;
        return view;
    }

    public void Close<T>() where T : UIView
    {
        if(openedViews.TryGetValue(typeof(T), out var view))
        {
            view.Close();
        }
    }

    public void Unload<T>() where T : UIView
    {
        if(openedViews.TryGetValue(typeof(T), out var view))
        {
            Destroy(view.gameObject);
            openedViews.Remove(typeof(T));
        }
        if(loadedHandles.TryGetValue(typeof(T), out var handle))
        {
            Addressables.Release(handle);
            loadedHandles.Remove(typeof(T));
        }
    }

#endregion
}
