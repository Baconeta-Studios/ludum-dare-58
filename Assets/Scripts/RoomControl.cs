using System.Collections.Generic;
using Coherence.Cloud;
using Coherence.Toolkit;
using System.Collections;
using UnityEngine;

public class RoomControl : MonoBehaviour
{
    public CoherenceBridge bridge;

    private CloudService cloudService;

    void Start()
    {
        cloudService = bridge.CloudService;
    }

    /// <summary>
    /// Unlists the current room so no new players can join (lock the room).
    /// Call this when your game session has started.
    /// </summary>
    public async void UnlistCurrentRoom(RoomData currentRoom)
    {
        IReadOnlyList<string> availableRegions = await cloudService.Rooms.RefreshRegionsAsync();
        CloudRoomsService roomsService = cloudService.Rooms.GetRoomServiceForRegion(availableRegions[0]);

        // Call unlist with UniqueId and Secret
        roomsService.UnlistRoom(currentRoom.UniqueId, currentRoom.Secret, response =>
        {
            if (response.Status == RequestStatus.Success)
            {
                Debug.Log("Room successfully unlisted — no new players can join!");
            }
            else
            {
                Debug.LogError("Failed to unlist room: " + response.Exception);
            }
        });
    }
}