using UnityEngine;
using System.Collections.Generic;

public class InfiniteBackground : MonoBehaviour
{
    public static InfiniteBackground Instance { get; private set; }

    // 무한맵 청크를 사용하기 위한 클래스
    [SerializeField] private GameObject tilePrefab;                 // 타일 프리팹
    [SerializeField] private Transform player;                      // 플레이어의 위치
    [SerializeField] private float tileSize = 20f;                  // 타일 한 변의 길이.
    [SerializeField] private int radius = 2;                        // 타일 반경.

    private Dictionary<Vector2Int, Transform> activeTiles = new();  // 활성화 된 타일 
    private Vector2Int lastPlayerCoord;                             // 플레이어의 이전 타일 좌표

    // 재사용을 위한 캐시 컬렉션
    private HashSet<Vector2Int> neededCoordsCache = new();
    private List<Vector2Int> toRecycleCache = new();
    private List<Vector2Int> toFillCache = new();

    private void Awake()
    {
        Instance = this;

        // 프리팹의 실제 바운즈에서 사이즈를 자동 계산
        Renderer renderer = tilePrefab.GetComponentInChildren<Renderer>();
        if(renderer != null)
        {   
            // 정사각형의 한변의 크기
            tileSize = renderer.bounds.size.x;
        }
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerSpawned += HandlePlayerSpanwed;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerSpawned -= HandlePlayerSpanwed;
    }

    private void HandlePlayerSpanwed(Transform player)
    {
        this.player = player;
        lastPlayerCoord = WorldToGridCoord(player.position);
        BuildInitialGrid(lastPlayerCoord);
    }

    private void Update()
    {
        if(player == null) return;

        Vector2Int newCoord = WorldToGridCoord(player.position);
        if(newCoord != lastPlayerCoord)
        {
            // 재배치 로직.
            Recenter(newCoord);
            lastPlayerCoord = newCoord;
        }
    }

    // 초기 타일을 구성하는 메서드
    private void BuildInitialGrid(Vector2Int center)
    {
        for(int x = -radius; x <= radius; ++x)
        {
            for(int z = -radius; z <= radius; ++z)
            {
                Vector2Int coord = center + new Vector2Int(x, z); 
                SpawnTileAt(coord);      
            }    
        }        
    }

    // 좌표에 타일을 설치하는 메서드
    private void SpawnTileAt(Vector2Int coord)
    {
        Vector3 worldPos = new Vector3(coord.x * tileSize, 0f, coord.y * tileSize);
        // 설치 로직.
        Transform tile = GameManager.Instance.PoolMgr.Get(tilePrefab).transform;
        tile.transform.position = worldPos;

        activeTiles[coord] = tile;
    }

    // 월드 좌표를 격자 좌표로 변환하는 메서드
    private Vector2Int WorldToGridCoord(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int z = Mathf.RoundToInt(worldPos.z / tileSize);

        return new Vector2Int(x, z);
    }

    private void Recenter(Vector2Int newCenter)
    {
        // 있어야 할 좌표 계산
        neededCoordsCache.Clear();
        for(int x = -radius; x <= radius; ++x)
        {
            for(int z = -radius; z <= radius; ++z)
            {
                neededCoordsCache.Add(newCenter + new Vector2Int(x, z));
            }   
        }

        // 범위 밖으로 나간 좌표 찾기
        toRecycleCache.Clear();
        foreach(var coord in activeTiles.Keys)
        {
            if(!neededCoordsCache.Contains(coord)) toRecycleCache.Add(coord);
        }

        toFillCache.Clear();
        foreach(var coord in neededCoordsCache)
        {
            if(!activeTiles.ContainsKey(coord)) toFillCache.Add(coord);
        }

        // 재활용 대상을 빈 자리로 1:1 이동
        int moveCount = Mathf.Min(toRecycleCache.Count, toFillCache.Count);
        for(int i = 0; i < moveCount; ++i)
        {
            Vector2Int oldCoord = toRecycleCache[i];
            Vector2Int newCoord = toFillCache[i];

            Transform tile = activeTiles[oldCoord];
            tile.position = new Vector3(newCoord.x * tileSize, 0f, newCoord.y * tileSize);

            activeTiles.Remove(oldCoord);
            activeTiles[newCoord] = tile;
        }

        for(int i = moveCount; i < toFillCache.Count; ++i)
        {
            SpawnTileAt(toFillCache[i]);
        }
    }


#region Public

    // 활성화된 타일 중에서 플레이어 주위(Margin) 1타일 내의 위치 확인
    public bool isWorldPositionOnActiveTile(Vector3 worldPos, int safetyMargin = 1)
    {
        // 활성화된 타일 위치가 아닌 경우
        Vector2Int coord = WorldToGridCoord(worldPos);
        if(!activeTiles.ContainsKey(coord)) return false;

        // 플레이어로 부터 얼마나 멀리 떨어져 있는지 체크
        Vector2Int offset = coord - lastPlayerCoord;
        int maxAllowed = radius - safetyMargin;

        return Mathf.Abs(offset.x) <= maxAllowed && Mathf.Abs(offset.y) <= maxAllowed;
    }

#endregion
}
