using System.Collections;
using System.Collections.Generic;
using Coherence.Toolkit;
using Managers;
using UnityEngine;

namespace Movement
{
    public class PlayerLogic : MonoBehaviour
    {
        private List<string> _myObjectives;

        private void Start()
        {
            if (TryGetComponent<CoherenceSync>(out var sync) && sync.HasStateAuthority)
            {
                Debug.Log("Sync object found- start player spawning system");
                GameManager.Instance.OnPlayerSpawned(name, gameObject);
                
                Invoke(nameof(SetupPlayer), 1f);
            }
        }

        private void SetupPlayer()
        {
            string playerId = name;
            
            _myObjectives = GameManager.Instance.GetMyObjectives(playerId);

            Debug.Log($"Player {playerId} objectives: {string.Join(", ", _myObjectives)}");
        }
    }
}