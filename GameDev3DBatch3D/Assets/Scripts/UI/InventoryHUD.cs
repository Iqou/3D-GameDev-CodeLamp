using UnityEngine;
using TMPro;

public class InventoryHUD : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private string prefix = "Food: ";
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color fullColor = Color.yellow;

    private void OnEnable()
    {
        if (inventory == null) return;

        inventory.OnChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (inventory == null) return;
        inventory .OnChanged -= Refresh;
    }

    private void Refresh()
    {
        label.text = prefix + inventory.Count + "/" + inventory.MaxCapacity;
        label.color = inventory.IsFull ? fullColor : normalColor;
    }
    
}
