using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootStrapper : MonoBehaviour
{
    // 각 매니저들의 인스턴스와 초기화 시점을 보장함.

    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InputManager inputManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 이미 GameManager가 존재한다면
        // BootStrapper가 다시 로드 되어 중복 초기화가 되는 것을 예방함.
        if(GameManager.Instance != null)
        {
            Destroy(gameObject);    // Bootstrapper 스스로를 파괴.
            return;
        }

        // 초기화 시점 강제.
        gameManager.RegisterAsInstance();
        uiManager.RegisterAsInstance();
        inputManager.RegisterAsInstance();
        
        gameManager.Init();
        inputManager.Init();

        InitializeUIAsync().Forget(ex => Debug.LogException(ex));
    }

    private async UniTask InitializeUIAsync()
    {
        await uiManager.PreloadAllAsync("UI");

#if UNITY_EDITOR
        await RedirectToEntrySceneInEditor();
#endif
    }

#if UNITY_EDITOR
    private const string EntrySceneKey = "PlayFromMainScene.EntryScene";

    private async UniTask RedirectToEntrySceneInEditor()
    {
        var entry = UnityEditor.SessionState.GetString(EntrySceneKey, "");
        if(string.IsNullOrEmpty(entry)) return;

        UnityEditor.SessionState.EraseString(EntrySceneKey);    // 1회성

        // 메인 씬이나 로딩 씬에서 Play한 경우에는 리다이렉트하지 않음.
        if (entry == SceneManager.GetActiveScene().name || entry == "Load_Scene") return;

        Debug.Log($"[Editor] '{entry}' 씬에서 Play -> 정상 로딩 흐름이로 진입");
        await gameManager.LoadMgr.SceneLoad(entry);
    }
#endif
}
