using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private Transform spawnPoint;

    private GameObject playerObj;

    private HUDController hud;

    public static event Action<Transform> OnPlayerSpawned;
    public static event Action<GameObject> GetPlayerObjAfterSpawned;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerObj = SpawnPlayer();
        BindPlayerAndHUD().Forget(ex => Debug.LogException(ex));
    }

    private async UniTask BindPlayerAndHUD()
    {
        hud = await UIManager.Instance.OpenAsync<HUDController>();

        hud.Bind(playerObj.GetComponent<PlayerAttack>());

        hud.gameObject.SetActive(true);
    }

    public GameObject SpawnPlayer()
    {
        var PlayerObject = GameManager.Instance.DataMgr.Get<GameObject>("Player");

        if(PlayerObject == null)
        {
            Debug.Log("[PlayerManager] 'Player'프리팹을 캐시에서 찾을 수 없습니다.");
            return null;
        }

        var instance = Instantiate(PlayerObject, spawnPoint.position, Quaternion.identity);

        OnPlayerSpawned?.Invoke(instance.transform);
        Debug.Log("Player Spawned");
        GetPlayerObjAfterSpawned?.Invoke(instance);

        return instance;
    }
}