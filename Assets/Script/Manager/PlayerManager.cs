using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    public Transform PlayerTransform { get; private set; }
    public GameObject PlayerObject { get; private set; }

    public static event Action<Transform> OnPlayerSpawned;

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
        PlayerObject = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        PlayerTransform = PlayerObject.transform;

        OnPlayerSpawned?.Invoke(PlayerTransform);
    }
}