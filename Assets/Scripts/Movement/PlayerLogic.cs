using System.Collections.Generic;
using Coherence.Toolkit;
using Managers;
using UnityEngine;

namespace Movement
{
    public class PlayerLogic : MonoBehaviour
    {
        private List<(string itemName, bool collected)> _myObjectives;

        private void Start()
        {
            if (TryGetComponent<CoherenceSync>(out var sync) && sync.HasStateAuthority)
            {
                Debug.Log("Sync object found- start player spawning system");
                GameManager.Instance.OnPlayerSpawned(name);
                
                Invoke(nameof(SetupPlayer), 1f);
            }
            else if (sync == null)
            {
                GameManager.Instance.OnPlayerSpawned(name);
            }
        }

        private void SetupPlayer()
        {
            string playerId = name;
            
            _myObjectives = GameManager.Instance.GetMyObjectivesForUI(playerId);

            Debug.Log($"Player {playerId} objectives: {string.Join(", ", _myObjectives)}");
        }
    }
}