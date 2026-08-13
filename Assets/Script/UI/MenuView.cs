using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;


public class MenuView : UIView
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button titleButton;


#region Init
    private void Awake()
    {
        settingButton.onClick.AddListener(OpenSettingPanel);
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }
#endregion

#region Method
    private void OpenSettingPanel()
    {
        UIManager.Instance.OpenAsync<SettingView>().Forget(ex => Debug.LogException(ex));
    }
#endregion
}