using UnityEngine;

public class FoodCrate : MonoBehaviour
{
    [SerializeField] private int foodAmount = 1;

    private void OnTriggerEnter(Collider other){
        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory == null) return;

        int accepted = inventory.TryAdd(foodAmount);
        Debug.Log($"Food Crate: Accepted {accepted} food. Remaining: {foodAmount - accepted}");
        if(accepted>= foodAmount){
            Destroy(gameObject);
        }
        else{
            foodAmount -= accepted;
        }
    }
}
