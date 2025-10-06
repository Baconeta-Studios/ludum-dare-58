using Coherence.Cloud;
using UnityEngine;

public class BaseSession : MonoBehaviour
{
    public static BaseSession Instance { get; private set; }
    public RoomData CurrentRoomData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}