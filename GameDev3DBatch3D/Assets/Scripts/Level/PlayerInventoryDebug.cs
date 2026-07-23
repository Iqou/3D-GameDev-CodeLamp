using UnityEngine;

public class PlayerInventoryDebug : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            int accepted = playerInventory.TryAdd(6);
            Debug.Log($"Tried to add 6 food. Accepted: {accepted}. Current count: {playerInventory.Count}/ Max: {playerInventory.Max}");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            int removed = playerInventory.TryRemove(6);
            Debug.Log($"Tried to remove 6 food. Removed: {removed}. Current count: {playerInventory.Count} / Max: {playerInventory.Max}");
        }
    }
}
