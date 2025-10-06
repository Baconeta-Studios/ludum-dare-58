using Coherence.Toolkit;
using Movement;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [Header("Networking")]
    [SerializeField] private CoherenceSync coherenceSync;

    [Header("AI Settings")]
    [SerializeField] private GameObject aiPrefab;
    [SerializeField, Min(1)] private int numberToSpawn = 1;

    [Header("Grid Spawn Settings")]
    [Tooltip("If true, only walkable cells will be considered.")]
    [SerializeField] private bool onlyWalkable = true;
    [Tooltip("If true, spawn in random grid cells. If false, spawn sequentially through grid.")]
    [SerializeField] private bool randomizeCells = true;
    [Tooltip("Optional Room ID filter (set -1 for any room).")]
    [SerializeField] private int roomFilter = -1;

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

        List<GridCell> validCells = new List<GridCell>();

        foreach (var cell in gridManager.Grid)
        {
            if (cell == null) continue;
            if (onlyWalkable && !cell.walkable) continue;
            if (roomFilter >= 0 && cell.roomID != roomFilter) continue;

            validCells.Add(cell);
        }

        if (validCells.Count == 0)
        {
            Debug.LogWarning("AIManager: No valid grid cells found for spawning.");
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
