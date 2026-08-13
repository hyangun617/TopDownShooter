using UnityEngine;
using System;
using System.Collections.Generic;

public enum PrefabType
{
    AudioClip,
    GameObject,
    Material
}

[Serializable]
public struct PrefabLabel
{
    [SerializeField] private PrefabType type;
    [SerializeField] private string label;

    public PrefabType Type => type;
    public string Label => label;
}

[CreateAssetMenu(fileName = "ScenePrefabs", menuName = "Scriptable Objects/Scene/ScenePrefabs")]
public class ScenePrefabs : ScriptableObject
{
    [SerializeField] private string sceneName;
    [SerializeField] private List<PrefabLabel> list;

    public string SceneName => sceneName;
    public List<PrefabLabel> List => list;
}
