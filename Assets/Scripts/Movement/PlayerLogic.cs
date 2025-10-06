using System;
using System.Collections.Generic;
using Coherence.Toolkit;
using Managers;
using UI;
using UnityEngine;

namespace Movement
{
    public class PlayerLogic : MonoBehaviour
    {
        private UIMurderabilia _myUIMurderabilia;
        
        private void OnEnable()
        {
            GameManager.ItemWasCollected += UpdateUIMurderabilia;
        }
        
        private void OnDisable()
        {
            GameManager.ItemWasCollected -= UpdateUIMurderabilia;
        }

        private List<(string itemName, bool collected)> _myObjectives;

        private void Start()
        {
            _myUIMurderabilia = FindAnyObjectByType<UIMurderabilia>();
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
            _myUIMurderabilia.UpdateCollectionUI(_myObjectives);
            Debug.Log($"Player {playerId} objectives: {string.Join(", ", _myObjectives)}");
        }

        private void UpdateUIMurderabilia()
        {
            var items = GameManager.Instance.GetMyObjectivesForUI(name);
            _myUIMurderabilia.UpdateCollectionUI(items);
        }
    }
}