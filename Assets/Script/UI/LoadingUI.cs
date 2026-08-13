using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingUI : UIView
{
    [SerializeField] private TMP_Text sceneName;            // 로드 되는 씬 이름
    [SerializeField] private Image sceneImage;              // 씬 이미지
    [SerializeField] private Image LoadingGauge;            // 로딩 바
    [SerializeField] private TMP_Text toolTip;              // 로딩 바 위 툴 팁
    [SerializeField] private TMP_Text currentState;         // 현재 로드 되는 Label

    void Awake()
    {
        LoadingGauge.fillAmount = 0f;
    }

    void Start()
    {
        LoadScene();
    }

    void OnEnable()
    {
        GameManager.Instance.LoadMgr.broadCastLoadState += UpdateLoadState;
    }

    void OnDisable()
    {
        GameManager.Instance.LoadMgr.broadCastLoadState -= UpdateLoadState;
    }

    private void UpdateLoadState(string label, LoadState state, float progress)
    {
        if(!string.IsNullOrEmpty(label))
            currentState.SetText($"{label}을 로드하는 중");
        
        LoadingGauge.fillAmount = progress;
    }

    // LoadManager에서 로드해야하는 씬 정보를 받아옴.
    public void LoadScene()
    {
        sceneName.SetText(GameManager.Instance.LoadMgr.GetLoadSceneName());
    }
}
