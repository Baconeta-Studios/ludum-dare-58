using Coherence.Toolkit;
using Movement;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [Header("Networking")]
    [SerializeField] private CoherenceSync coherenceSync;

    [Header("AI Settings")]
    [SerializeField] private GameObject aiPrefab;
    [SerializeField, Min(1)] private int numberToSpawn = 1;

    [Header("Parenting")]
    [SerializeField] private Transform aiParent;

    private GridManager gridManager;

    private void Awake()
    {
        // Auto-find grid in the scene
        gridManager = FindFirstObjectByType<GridManager>();

        if (gridManager == null)
        {
            Debug.LogError("AIManager: No GridManager found in the scene!");
        }
    }

    private void Start()
    {
        if (coherenceSync != null && coherenceSync.HasStateAuthority)
        {
            Debug.Log("AIManager: I have state authority, spawning AI...");
            SpawnOnGrid();
        }
        else
        {
            Debug.Log("AIManager: I’m a client, not spawning AI.");
        }
    }

    private void SpawnOnGrid()
    {
        if (gridManager == null || gridManager.Grid == null)
        {
            Debug.LogWarning("AIManager: GridManager not ready.");
            return;
        }

        for (var i = 0; i < numberToSpawn; i++)
        {
            SpawnOne();
        }
    }

    private void SpawnOne()
    {
        // location based spawning is not working as expected
        var ai = Instantiate(aiPrefab);
    }
}
