using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    private GameObject playerObj;

    public static event Action<Transform> OnPlayerSpawned;
    public static event Action<GameObject> GetPlayerObjAfterSpawned;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerObj = SpawnPlayer();

        UIManager.Instance.PreloadCompleted += BindPlayerAndHUD;
    }

    private async void BindPlayerAndHUD()
    {
        var hud = await UIManager.Instance.OpenAsync<HUDController>();

        hud.Bind(playerObj.GetComponent<PlayerAttack>());

        UIManager.Instance.PreloadCompleted -= BindPlayerAndHUD;
    }

    public GameObject SpawnPlayer()
    {
        var PlayerObject = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

        OnPlayerSpawned?.Invoke(PlayerObject.transform);
        Debug.Log("Player Spawned");
        GetPlayerObjAfterSpawned?.Invoke(PlayerObject);

        return PlayerObject;
    }
}