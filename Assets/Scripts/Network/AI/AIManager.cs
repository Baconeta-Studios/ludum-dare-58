using Coherence.Toolkit;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField]
    public CoherenceSync coherenceSync;

    public GameObject aiPrefab; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (coherenceSync.HasStateAuthority)
        {
            // Ignore for Simulators and hosts.
            Debug.Log("I'm the server");
            Instantiate(aiPrefab);
        } else {
            Debug.Log("I'm the client");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
