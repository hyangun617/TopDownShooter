using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리팹을 키로 사용해 풀을 구분한다.
    private Dictionary<GameObject, ObjectPool<GameObject>> pools
        = new Dictionary<GameObject, ObjectPool<GameObject>>();

    private ObjectPool<GameObject> GetOrCreatePool(GameObject prefab, int InitSize = 20)
    {
        if(!pools.TryGetValue(prefab, out var pool))
        {
            pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(prefab);
                    if(obj.TryGetComponent<Enemy>(out var enemy))
                        enemy.EnsureSetup();
                    return obj;
                },
                actionGet: obj => obj.SetActive(true),
                actionRelease: obj => obj.SetActive(false),
                InitSize: InitSize
            );
            pools.Add(prefab, pool);
        }
        return pool;
    }

    public GameObject Get(GameObject prefab)
    {
        GameObject instance = GetOrCreatePool(prefab).Get();
        
        if(instance.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.SourcePrefab = prefab;
            poolable.OnSpawn();
        }
        return instance;
    }

    public void Release(GameObject instance)
    {
        if(instance.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.OnDespawn();
            GetOrCreatePool(poolable.SourcePrefab).Release(instance);
        }
        else
        {
            Destroy(instance);
        }
    }

    public void ClearAll()
    {
        foreach(var pool in pools.Values)
        {
            pool.Clear(obj => Destroy(obj));
        }
        pools.Clear();
    }
}