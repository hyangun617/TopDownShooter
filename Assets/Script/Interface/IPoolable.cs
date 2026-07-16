using UnityEngine;

public interface IPoolable
{
    GameObject SourcePrefab { get; set; }
    void OnSpawn();
    void OnDespawn();
}