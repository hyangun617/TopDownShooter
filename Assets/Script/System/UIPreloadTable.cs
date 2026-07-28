using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.AddressableAssets;

public enum UIViewID
{
    Menu,
    Settings
}

[Serializable]
public class UIPreloadEntry
{
    public UIViewID viewID;
    public AssetReferenceGameObject prefabRef;      // 인스펙터에서 드래그 앤 드롭 가능.
}

[CreateAssetMenu(menuName = "UI/UIPreloadTable")]
public class UIPreloadTable : ScriptableObject
{
    public List<UIPreloadEntry> entries;
}
