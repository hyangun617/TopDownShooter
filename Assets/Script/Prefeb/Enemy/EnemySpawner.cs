using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefab;
    [SerializeField] private Transform player;


    // Enemy의 스폰 범위.
    [Header("Spawn Range")]
    [SerializeField] private float minSpawnRadius = 8f;
    [SerializeField] private float maxSpawnRadius = 15f;

    // Enemy의 스폰 주기
    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 3f;
    // 최대 적 개수.
    [SerializeField] private int maxAliveCount = 10;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask ObstacleLayerMask;

    // 현재 생성되어 살아있는 적 개수
    private int currentAliveCount = 0;

    // 충돌체 버퍼
    private Collider[] buffers = new Collider[10];

    private void Awake()
    {
        if(ObstacleLayerMask == 0)
        {
            Debug.LogError($"{name} : OBstacleLayerMask가 Inspector에서 설정되지 않았습니다.");
        }        
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private void OnEnable()
    {
        Enemy.OnAnyEnemyDeath += OnEnemyDeath;
    }

    // 주기적 적 생성.
    private IEnumerator SpawnLoop()
    {
        yield return new WaitUntil(() => GameManager.Instance.DataMgr.IsDataInitialized);

        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while(true)
        {
            yield return wait;

            if(currentAliveCount >= maxAliveCount) continue;

            TrySpawnEnemy();
        }
    }

    private bool TrySpawnEnemy()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();

        int hitCount = Physics.OverlapSphereNonAlloc(spawnPos, 1f, buffers, ObstacleLayerMask);

        if(hitCount == 0)
        {
           SpawnEnemy(spawnPos);
           return true; 
        }
        
        return false;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // 랜덤 방향, 거리 
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float RandomDist = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector3 offset = new Vector3(randomDir.x, 0f, randomDir.y) * RandomDist;
        return player.position + offset;
    }

    private void SpawnEnemy(Vector3 pos)
    {
        GameObject enemyObj = GameManager.Instance.PoolMgr.Get(enemyPrefab[Random.Range(0, enemyPrefab.Count)]);
        enemyObj.transform.position = pos;
        enemyObj.transform.rotation = Quaternion.identity;

        currentAliveCount++;
    }

    private void OnEnemyDeath(Enemy enemy) => currentAliveCount--;

    private void OnDisable()
    {
        Enemy.OnAnyEnemyDeath -= OnEnemyDeath;
    }
}
