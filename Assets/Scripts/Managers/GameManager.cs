using System;
using System.Collections.Generic;
using System.Linq;
using Coherence.Toolkit;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    [System.Serializable]
    public class Objective
    {
        public string PrefabId;
        public bool Collected;

        public Objective(string id)
        {
            PrefabId = id;
            Collected = false;
        }
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Config")] [SerializeField] private int objectivesPerPlayer = 3;

        [Tooltip("All collectible items in the level (assign in inspector or find at runtime)")] [SerializeField]
        private List<CollectibleItem> allItems = new();

        [Tooltip("All enemies in the level (stub for now)")] [SerializeField]
        private List<GameObject> allEnemies = new();

        private Dictionary<string, List<Objective>> playerObjectives = new();
        private HashSet<string> collectedItemIds = new();
        private HashSet<string> deadEnemies = new();
        private Queue<CollectibleItem> unassignedItems = new();
        private List<CollectibleItem> assignedItems = new();

        private CoherenceSync _sync;
        
        public static event Action ItemWasCollected;

        private void Awake()
        {
            Instance = this;
            TryGetComponent(out _sync);
        }

        private void Start()
        {
            Debug.Log("GameManager started. Items: " + allItems.Count +
                      " | Enemies: " + allEnemies.Count);
            unassignedItems = new Queue<CollectibleItem>(allItems);
        }

        public CollectibleItem GetNextItemForAI()
        {
            if (unassignedItems.Count > 0)
            {
                var item = unassignedItems.Dequeue();
                Debug.Log($"Assigning {item.ItemName} to AI");
                assignedItems.Add(item);
                return item;
            }

            Debug.LogWarning("No more items left to assign to AI!");
            return null;
        }


        public void OnPlayerSpawned(string playerId)
        {
            if (!IsAuthority())
                return;

            Debug.Log($"Player {playerId} spawned!");

            if (!playerObjectives.ContainsKey(playerId))
            {
                playerObjectives[playerId] = AssignObjectives();
            }

            // Convert IDs back to display names for logging
            var objectiveNames = new List<string>();
            foreach (var obj in playerObjectives[playerId])
            {
                var item = allItems.Find(i => i.PrefabId == obj.PrefabId);
                objectiveNames.Add(item != null ? item.ItemName : $"Unknown({obj.PrefabId})");
            }

            Debug.Log($"Assigned objectives for {playerId}: {string.Join(", ", objectiveNames)}");
        }

        private List<Objective> AssignObjectives()
        {
            var result = new List<Objective>();
            var pool = new List<CollectibleItem>(allItems);

            for (var i = 0; i < objectivesPerPlayer && pool.Count > 0; i++)
            {
                int index = Random.Range(0, pool.Count);
                result.Add(new Objective(pool[index].PrefabId));
                pool.RemoveAt(index);
            }

            return result;
        }

        public void CollectItem(string playerId, CollectibleItem item)
        {
            if (!IsAuthority() || item == null)
                return;

            Debug.Log($"Player {playerId} collected {item.ItemName} (prefabId={item.PrefabId})");
            collectedItemIds.Add(item.PrefabId);

            if (playerObjectives.TryGetValue(playerId, out var objectives))
            {
                var objective = objectives.Find(o => o.PrefabId == item.PrefabId);
                if (objective is { Collected: false })
                {
                    objective.Collected = true;
                    Debug.Log($"Player {playerId} marked {item.ItemName} as collected");
                }
                else
                {
                    Debug.Log($"Player {playerId} collected {item.ItemName}, but it wasn’t on their list.");
                }
            }

            ItemWasCollected?.Invoke();

            item.gameObject.SetActive(false);

            PrintDebugState();
            CheckForGameOver();
        }


        public void KillEnemy(string enemyId)
        {
            if (!IsAuthority())
                return;

            Debug.Log($"Enemy {enemyId} killed");
            deadEnemies.Add(enemyId);

            PrintDebugState();
        }

        private void PrintDebugState()
        {
            Debug.Log("---- DEBUG STATE ----");
            foreach (var kvp in playerObjectives)
            {
                var objectiveStates = new List<string>();
                foreach (var obj in kvp.Value)
                {
                    var prefab = allItems.Find(i => i.PrefabId == obj.PrefabId);
                    string display = prefab != null ? prefab.ItemName : $"Unknown({obj.PrefabId})";
                    if (obj.Collected) display = $"~~{display}~~"; // markdown-style strikeout
                    objectiveStates.Add(display);
                }

                Debug.Log(
                    $"Player {kvp.Key}: {objectiveStates.Count} objectives ({string.Join(", ", objectiveStates)})");
            }

            var collectedNames = new List<string>();
            foreach (var id in collectedItemIds)
            {
                var prefab = allItems.Find(i => i.PrefabId == id);
                collectedNames.Add(prefab != null ? prefab.ItemName : $"Unknown({id})");
            }

            Debug.Log("Collected items: " + string.Join(", ", collectedNames));
            Debug.Log("Dead enemies: " + string.Join(", ", deadEnemies));
            Debug.Log("---------------------");
        }

        private bool IsAuthority()
        {
            return _sync == null || _sync.HasStateAuthority;
        }

        // Now returns names + collected status for UI
        public List<(string itemName, bool collected)> GetMyObjectivesForUI(string playerId)
        {
            var result = new List<(string, bool)>();

            if (playerObjectives.TryGetValue(playerId, out var objectives))
            {
                foreach (var obj in objectives)
                {
                    var prefab = allItems.Find(i => i.PrefabId == obj.PrefabId);
                    string name = prefab != null ? prefab.ItemName : $"Unknown({obj.PrefabId})";
                    result.Add((name, obj.Collected));
                }
            }

            return result;
        }

        private void CheckForGameOver()
        {
            foreach (var kvp in playerObjectives)
            {
                var playerId = kvp.Key;
                var objectives = kvp.Value;

                bool allCollected = objectives.TrueForAll(o => o.Collected);

                if (allCollected)
                {
                    Debug.Log($"GAME OVER! Player {playerId} has completed all objectives!");
                    // TODO actual winner will be highest score (probably first to collect all)
                    EndGame(playerId);
                    return;
                }
            }

            if (AllItemsCollected())
            {
                Debug.Log($"GAME OVER! All items have been collected!");
                // TODO actual winner will be highest score (probably first to collect all)
                EndGame("todo");
            }
        }

        private void EndGame(string winnerId)
        {
            Debug.Log($"==> GAME ENDED. Winner: {winnerId}");

            // TODO: stop player input, play cutscene, fade out, load end scene, etc.
        }

        private bool AllItemsCollected()
        {
            return assignedItems.All(item => collectedItemIds.Contains(item.PrefabId));
        }

        public void MurderWasWitnessed(string murdererId, string witnessId)
        {
            if (!IsAuthority())
            {
                Debug.Log("Murder was witnessed but somehow this was not the server auth");
                return;
            }

            Debug.LogWarning($"Murder was witnessed! Murderer={murdererId}, Witness={witnessId}");

            //EndGame(murdererId); <== server authorise something spicy
        }
    }
}