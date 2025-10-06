using System.Collections.Generic;
using System.Threading.Tasks;
using Coherence.Cloud;
using Coherence.Toolkit;
using UnityEngine;
using UnityEngine.InputSystem;


public class DoorClickHandler : MonoBehaviour
{
    // Assign this script to your Door object (or a manager if you prefer).
    // Option A: attach directly to Door and check hit
    // Option B: attach to a central "InputManager" that detects clicks and routes them
    private CoherenceBridge _bridge;
    private CloudService _cloud;

    public RoomControl roomControl;   // drag your RoomControl GameObject here in the inspector

    void Awake()
    {
        _bridge = FindObjectOfType<CoherenceBridge>();
        _cloud = _bridge.CloudService;
        // RoomData currentRoom = _bridge.CloudService.CurrentRoom;
    }

    void Update()
    {
        // Handle left mouse/touch click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckForClick();
        }

        // (Optional) Handle touch input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            CheckForClick();
        }
    }

    private void CheckForClick()
    {
        // 1. Get mouse/touch position
        Vector2 screenPosition = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Touchscreen.current.primaryTouch.position.ReadValue();

        // 2. Convert screen position into a ray
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        // 3. Raycast into the world
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 4. Check if we hit THIS door
            if (hit.collider.gameObject == gameObject)
            {
                OnDoorClicked();
            }

            // Alternative: check if object implements an interface
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                clickable.OnClick();
            }
        }
    }

        // Called if this door was directly hit
    private async void OnDoorClicked()
    {
        Debug.Log("Door clicked!");

        var baseSession = FindFirstObjectByType<BaseSession>();
      
        await UnlistRoom(baseSession.CurrentRoomData);
        // TODO destroy door Destroy(this);
    }

    private async Task UnlistRoom(RoomData currentRoomData)
    {
        // Get the room from the list of assumed rooms. 

        IReadOnlyList<string> availableRegions = await _cloud.Rooms.RefreshRegionsAsync();
        CloudRoomsService roomsService = _cloud.Rooms.GetRoomServiceForRegion(availableRegions[0]);

        // Call unlist with UniqueId and Secret
        roomsService.UnlistRoom(currentRoomData.UniqueId, currentRoomData.Secret, response =>
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
    // Example interface for generic clickable objects
    public interface IClickable
    {
        void OnClick();
    }
}

  
