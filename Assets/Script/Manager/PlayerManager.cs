using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    public static event Action<Transform> OnPlayerSpawned;
    public static event Action<GameObject> GetPlayerObjAfterSpawned;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        var PlayerObject = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

        OnPlayerSpawned?.Invoke(PlayerObject.transform);
        Debug.Log("Player Spawned");
        GetPlayerObjAfterSpawned?.Invoke(PlayerObject);
    }
}