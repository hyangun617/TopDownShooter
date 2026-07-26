using System;

public class SettingManager
{
    private SoundManager soundMgr;

    public float MasterVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;
    public float BGMVolume { get; private set; } = 1f;
    
    public SettingManager(SoundManager soundManager) => soundMgr = soundManager;

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        soundMgr.SetMasterVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = value;
        soundMgr.SetSFXVolume(value);
    }

    public void SetBGMVolume(float value)
    {
        BGMVolume = value;
        soundMgr.SetBGMVolume(value);
    }
}