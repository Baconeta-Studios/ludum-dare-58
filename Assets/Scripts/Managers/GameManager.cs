using System.Collections.Generic;
using Coherence.Toolkit;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Config")]
        [SerializeField] private int objectivesPerPlayer = 3;

        [Tooltip("All collectible items in the level (assign in inspector or find at runtime)")]
        [SerializeField] private List<CollectibleItem> allItems = new();

        [Tooltip("All enemies in the level (stub for now)")]
        [SerializeField] private List<GameObject> allEnemies = new();

        private Dictionary<string, List<string>> playerObjectives = new();
        private HashSet<string> collectedItemIds = new(); 
        private HashSet<string> deadEnemies = new();

        private CoherenceSync _sync;

        private void Awake()
        {
            Instance = this;
            TryGetComponent(out _sync);
        }

        private void Start()
        {
            Debug.Log("GameManager started. Items: " + allItems.Count +
                      " | Enemies: " + allEnemies.Count);
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
            foreach (var id in playerObjectives[playerId])
            {
                var item = allItems.Find(i => i.PrefabId == id);
                objectiveNames.Add(item != null ? item.ItemName : $"Unknown({id})");
            }

            Debug.Log($"Assigned objectives for {playerId}: {string.Join(", ", objectiveNames)}");
        }


        private List<string> AssignObjectives()
        {
            var result = new List<string>();
            var pool = new List<CollectibleItem>(allItems); // catalog of prefabs

            for (var i = 0; i < objectivesPerPlayer && pool.Count > 0; i++)
            {
                int index = Random.Range(0, pool.Count);
                result.Add(pool[index].PrefabId); // store type ID
                pool.RemoveAt(index);
            }

            return result;
        }


        public void CollectItem(string playerId, CollectibleItem item)
        {
            if (!IsAuthority())
                return;

            Debug.Log($"Player {playerId} collected {item.ItemName} (prefabId={item.PrefabId})");
            collectedItemIds.Add(item.PrefabId);

            if (playerObjectives.ContainsKey(playerId) && playerObjectives[playerId].Contains(item.PrefabId))
            {
                playerObjectives[playerId].Remove(item.PrefabId);
                Debug.Log($"Player {playerId} objectives left: {string.Join(", ", playerObjectives[playerId])}");
            }
            else
            {
                Debug.Log($"Player {playerId} collected {item.ItemName}, but it wasn’t on their list.");
            }

            item.gameObject.SetActive(false);
            PrintDebugState();
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
                var objectiveNames = new List<string>();
                foreach (var id in kvp.Value)
                {
                    var prefab = allItems.Find(i => i.PrefabId == id);
                    objectiveNames.Add(prefab != null ? prefab.ItemName : $"Unknown({id})");
                }

                Debug.Log($"Player {kvp.Key}: {kvp.Value.Count} objectives left ({string.Join(", ", objectiveNames)})");
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

        public List<string> GetMyObjectivesForUI(string playerId)
        {
            var names = new List<string>();
            if (playerObjectives.TryGetValue(playerId, out var ids))
            {
                foreach (var id in ids)
                {
                    var item = allItems.Find(i => i.PrefabId == id);
                    if (item != null) names.Add(item.ItemName);
                }
            }
            return names;
        }
    }
}
