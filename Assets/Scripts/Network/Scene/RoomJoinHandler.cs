using Coherence.Cloud;
using Coherence.Toolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RoomJoinHandler : MonoBehaviour
{
    public CoherenceBridge bridge;
    public SceneAsset sceneToLoad;
    void Awake()
    {
        bridge.onConnected.AddListener(OnBridgeConnected);
    }

    void OnDestroy()
    {
        bridge.onConnected.RemoveListener(OnBridgeConnected);
    }

    public void Join(RoomData room)
    {
        bridge.JoinRoom(room);
    }

    private void OnBridgeConnected(CoherenceBridge b)
    {
        Debug.Log("Bridge connected — likely in room now");
        // Optionally verify room state here
        SceneManager.LoadScene(sceneToLoad.name);
    }
}
