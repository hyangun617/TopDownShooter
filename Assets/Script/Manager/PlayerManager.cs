using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
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
        BindPlayerAndHUD();
        hud.gameObject.SetActive(true);
    }

    private async void BindPlayerAndHUD()
    {
        hud = await UIManager.Instance.OpenAsync<HUDController>();

        hud.Bind(playerObj.GetComponent<PlayerAttack>());
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