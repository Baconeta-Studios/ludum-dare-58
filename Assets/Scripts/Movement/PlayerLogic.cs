using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Movement
{
    public class PlayerLogic : MonoBehaviour
    {
        private List<string> _myObjectives;
        private void Start()
        {
            Debug.Log("Start player spawning system");
            GameManager.Instance.OnPlayerSpawned(name, gameObject);
            
            Invoke(nameof(GetObjectives), 1f);
        }

        private void GetObjectives()
        {
            _myObjectives = GameManager.Instance.GetMyObjectives(name);
        }
    }
}