using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[Serializable]
public class TabEntry
{
    public Toggle toggle;
    public UIPanel panel;
}


public class TapController : MonoBehaviour
{
    [SerializeField] private ToggleGroup tabGroup;
    [SerializeField] private List<TabEntry> tabs;
    private Dictionary<Toggle, UIPanel> ToggleToPanel;

    private void Awake()
    {
        ToggleToPanel = new Dictionary<Toggle, UIPanel>();
        tabGroup = GetComponent<ToggleGroup>();

        foreach(var tab in tabs)
        {
            ToggleToPanel[tab.toggle] = tab.panel;
            tab.toggle.group = tabGroup;
            tab.toggle.onValueChanged.AddListener((isOn) => OnTabClicked(isOn, tab.panel));
        }
    }

    private void OnTabClicked(bool isClicked, UIPanel uiPanel)
    {
        if(isClicked)
        {
            uiPanel.Show();
        }
        else
        {
            uiPanel.Hide();
        }
    }
}
