using Coherence.Connection;
using Coherence.Toolkit;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject prefabToSpawn;
    public Vector3 initialPosition = Vector3.zero;

    private CoherenceBridge _bridge;
    private GameObject _player;

    private void Awake()
    {
        _bridge = FindFirstObjectByType<CoherenceBridge>();
        if (_bridge == null)
        {
            Debug.LogError("CoherenceBridge not found in the scene.");
            return;
        }
        this.OnConnection(_bridge);
        _bridge.onConnected.AddListener(OnConnection);
        Debug.LogWarning("Bridge Mounted On Connection listener");
        _bridge.onDisconnected.AddListener(OnDisconnection);
    }

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        if (_bridge != null)
        {
            _bridge.onConnected.RemoveListener(OnConnection);
            _bridge.onDisconnected.RemoveListener(OnDisconnection);
        }
    }

    // Handle local player lifetime
    private void OnConnection(CoherenceBridge bridge)
    {
        Debug.LogWarning("New Connection...");
        SpawnPlayer();
    }

    private void OnDisconnection(CoherenceBridge bridge, ConnectionCloseReason reason)
    {
        DespawnPlayer();
    }

    private void SpawnPlayer()
    {
        Debug.LogWarning("Spawning player....");
        if (_player != null)
        {
            Debug.LogWarning("Player already spawned.");
            return;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError("PrefabToSpawn is not assigned!");
            return;
        }

        _player = Instantiate(prefabToSpawn, initialPosition, Quaternion.identity);
        Debug.LogWarning("Player Spawned");
    }

    private void DespawnPlayer()
    {
        if (_player != null)
        {
            Destroy(_player);
            _player = null;
        }
    }
}