using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private string prefabId; // stable ID for prefab type

    public string ItemName => itemName;
    public string PrefabId => prefabId;

    private void Reset()
    {
        // Auto-assign prefabId in editor if empty
        if (string.IsNullOrEmpty(prefabId))
        {
            prefabId = System.Guid.NewGuid().ToString();
        }
    }
}