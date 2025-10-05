using System.Collections.Generic;
using Coherence.Toolkit;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Debug Config")] [SerializeField]
        private int objectivesPerPlayer = 3;

        [SerializeField]
        private List<string> allItemIds = new List<string>() { "item_1", "item_2", "item_3", "item_4" };

        [SerializeField] private List<string> allEnemyIds = new List<string>() { "enemy_1", "enemy_2" };

        private Dictionary<string, List<string>> playerObjectives = new();
        private HashSet<string> collectedItems = new(); // global log only
        private HashSet<string> deadEnemies = new();

        private CoherenceSync _sync;

        private void Awake()
        {
            Instance = this;

            TryGetComponent(out _sync);
        }

        private void Start()
        {
            Debug.Log("GameManager started. Items: " + string.Join(", ", allItemIds) +
                      " | Enemies: " + string.Join(", ", allEnemyIds));
        }

        public void OnPlayerSpawned(string playerId, GameObject playerObj)
        {
            if (!IsAuthority())
                return;

            Debug.Log($"Player {playerId} spawned!");

            if (!playerObjectives.ContainsKey(playerId))
            {
                playerObjectives[playerId] = AssignObjectives();
            }

            Debug.Log($"Assigned objectives for {playerId}: {string.Join(", ", playerObjectives[playerId])}");
        }

        private List<string> AssignObjectives()
        {
            var result = new List<string>();

            for (var i = 0; i < objectivesPerPlayer; i++)
            {
                var index = Random.Range(0, allItemIds.Count);
                result.Add(allItemIds[index]); // allow overlap
            }

            return result;
        }

        public void CollectItem(string playerId, string itemId)
        {
            if (!IsAuthority())
                return;

            Debug.Log($"Player {playerId} collected {itemId}");
            collectedItems.Add(itemId);

            if (playerObjectives.ContainsKey(playerId) && playerObjectives[playerId].Contains(itemId))
            {
                playerObjectives[playerId].Remove(itemId);
                Debug.Log($"Player {playerId} objectives left: {string.Join(", ", playerObjectives[playerId])}");
            }
            else
            {
                Debug.Log($"Player {playerId} collected {itemId}, but it wasn’t on their list.");
            }

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
                Debug.Log($"Player {kvp.Key}: {kvp.Value.Count} objectives left ({string.Join(", ", kvp.Value)})");
            }

            Debug.Log("Collected items (global log): " + string.Join(", ", collectedItems));
            Debug.Log("Dead enemies: " + string.Join(", ", deadEnemies));
            Debug.Log("---------------------");
        }

        private bool IsAuthority()
        {
            return _sync == null || _sync.HasStateAuthority;
        }

        public List<string> GetMyObjectives(string playerId)
        {
            foreach (var kvp in playerObjectives)
            {
                if (kvp.Key.Contains(playerId))
                {
                    return kvp.Value;
                }
            }
            Debug.Log("Somehow no objectives exist for player " + playerId);
            return null;
        }
    }
}