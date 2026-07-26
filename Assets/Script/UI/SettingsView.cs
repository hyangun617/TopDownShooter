using UnityEngine;
using UnityEngine.UI;

public class SettingsView : UIView
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;

    #region EventListener

    private void Awake()
    {
        masterVolumeSlider.onValueChanged.AddListener(GameManager.Instance.SettingMgr.SetMasterVolume);
        sfxVolumeSlider.onValueChanged.AddListener(GameManager.Instance.SettingMgr.SetSFXVolume);
        bgmVolumeSlider.onValueChanged.AddListener(GameManager.Instance.SettingMgr.SetBGMVolume);
    }

    #endregion

    public override void Open()
    {
        base.Open();

        masterVolumeSlider.SetValueWithoutNotify(GameManager.Instance.SettingMgr.MasterVolume);
        sfxVolumeSlider.SetValueWithoutNotify(GameManager.Instance.SettingMgr.SFXVolume);
        bgmVolumeSlider.SetValueWithoutNotify(GameManager.Instance.SettingMgr.BGMVolume);

    }
}