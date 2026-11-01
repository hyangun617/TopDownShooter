using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class PlayFromMainScene
{
    private const string MainScenePath = "Assets/Scenes/Title_Scene.unity"; // 실제 경로로 수정
    private const string MenuPath = "Tools/Play/Always Boot From Main Scene";
    private const string PrefKey = "PlayFromMainScene.Enabled";
    private const string EntrySceneKey = "PlayFromMainScene.EntryScene"; // BootStrapper와 같은 문자열

    static PlayFromMainScene()
    {
        Apply(Enabled);

        // Play 직전에, 지금 편집 중인 씬 이름을 기록
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.ExitingEditMode)
                SessionState.SetString(EntrySceneKey, EditorSceneManager.GetActiveScene().name);
        };
    }

    private static bool Enabled
    {
        get => EditorPrefs.GetBool(PrefKey, false);
        set => EditorPrefs.SetBool(PrefKey, value);
    }

    private static void Apply(bool on)
    {
        EditorSceneManager.playModeStartScene =
            on ? AssetDatabase.LoadAssetAtPath<SceneAsset>(MainScenePath) : null;
    }

    [MenuItem(MenuPath)]
    private static void Toggle()
    {
        Enabled = !Enabled;
        Apply(Enabled);
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MenuPath, Enabled);
        return true;
    }
}