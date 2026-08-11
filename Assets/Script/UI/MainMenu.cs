using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartClicked);
        settingButton.onClick.AddListener(OnSettingClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStartClicked);
        settingButton.onClick.RemoveListener(OnSettingClicked);
        quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    private void OnStartClicked()
    {
        
    }

    private void OnSettingClicked()
    {
        
    }

    // 게임 종료
    private void OnQuitClicked() => Application.Quit();
}
