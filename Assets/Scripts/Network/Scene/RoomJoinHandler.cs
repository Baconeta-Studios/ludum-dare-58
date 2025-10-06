using Coherence.Cloud;
using Coherence.Toolkit;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;
    
public class RoomJoinHandler : MonoBehaviour
{
    public CoherenceBridge bridge;
    public SceneReference scene;

    private void Awake()
    {
        bridge.onConnected.AddListener(OnBridgeConnected);
    }

    private void OnDestroy()
    {
        bridge.onConnected.RemoveListener(OnBridgeConnected);
    }

    public void Join(RoomData room)
    {
        bridge.JoinRoom(room);
    }

    private void OnBridgeConnected(CoherenceBridge b)
    {
        Debug.Log("Bridge connected � likely in room now");
        // Optionally verify room state here
        SceneManager.LoadScene(scene.Name);
    }
}
