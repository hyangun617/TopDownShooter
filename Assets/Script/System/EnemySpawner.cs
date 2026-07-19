using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefab;
    [SerializeField] private List<bool> isCanSpawnIndex;

    [Header("Player")]
    [SerializeField] private Transform player;

    // Enemy의 스폰 범위 및 주기.
    [Header("Enemy Spawner Setting")]
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private float minSpawnRadius = 8f;
    [SerializeField] private float maxSpawnRadius = 15f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxAliveCount = 10;                // 최대 적 개수.
    [SerializeField] private int maxAttemps = 3;                    // 최대 재시도 회수
    
    [Header("Layer Mask")]
    [SerializeField] private LayerMask ObstacleLayerMask;

    // 현재 생성되어 살아있는 적 개수
    private int currentAliveCount = 0;

    // 스폰 Enemy 인덱스
    private int enemyIndex = 0;

    // 충돌체 버퍼
    private Collider[] buffers = new Collider[10];

    public void PauseSpawner() => isEnabled = false;
    public void ResumeSpawner() => isEnabled = true;

    private void Awake()
    {
        if(ObstacleLayerMask == 0)
        {
            Debug.LogError($"{name} : ObstacleLayerMask가 Inspector에서 설정되지 않았습니다.");
        }        
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());

        for (int i = 0; i < enemyPrefab.Count; i++)
        {
            isCanSpawnIndex.Add(true);
        }
    }

    private void OnEnable()
    {
        Enemy.OnAnyEnemyDeath += OnEnemyDeath;
        PlayerManager.OnPlayerSpawned += SetupPlayer;
    }

    private void OnDisable() 
    {
        Enemy.OnAnyEnemyDeath -= OnEnemyDeath;
        PlayerManager.OnPlayerSpawned -= SetupPlayer;
    }

    // 주기적 적 생성.
    private IEnumerator SpawnLoop()
    {
        yield return new WaitUntil(() => GameManager.Instance.DataMgr.IsDataInitialized);

        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while(true)
        {
            yield return wait;

            if(!isEnabled) yield return new WaitUntil(() => isEnabled);
            if(currentAliveCount >= maxAliveCount) continue;

            TrySpawnEnemy();
        }
    }

    private bool TrySpawnEnemy()
    {
        for(int i = 0; i < maxAttemps; ++i)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();

            // 해당 위치가 유효한지 확인
            if(!InfiniteBackground.Instance.isWorldPositionOnActiveTile(spawnPos)) continue;

            // 해당 위치의 장애물 확인
            int hitCount = Physics.OverlapSphereNonAlloc(spawnPos, 1f, buffers, ObstacleLayerMask);
            if(hitCount == 0)
            {
                // 랜덤 인덱스 생성 후 인덱스에 해당하는 프리팹의 생성 가능 여부 확인.
                enemyIndex = Random.Range(0, enemyPrefab.Count);

                if(isCanSpawnIndex[enemyIndex])
                {
                    SpawnEnemy(spawnPos, enemyIndex);
                    return true;     
                }                
            }    
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

    private void SpawnEnemy(Vector3 pos, int index)
    {
        GameObject enemyObj = GameManager.Instance.PoolMgr.Get(enemyPrefab[index]);
        enemyObj.transform.position = pos;
        enemyObj.transform.rotation = Quaternion.identity;

        currentAliveCount++;
    }

    private void OnEnemyDeath(Enemy enemy) => currentAliveCount--;
    private void SetupPlayer(Transform player) => this.player = player;
}
