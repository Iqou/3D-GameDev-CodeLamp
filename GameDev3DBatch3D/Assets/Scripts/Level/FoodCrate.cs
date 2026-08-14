using System;
using UnityEngine;

public class FoodCrate : MonoBehaviour
{
    [SerializeField] private int foodAmount = 1;

    public event Action<FoodCrate> OnCollected;
    private void OnTriggerEnter(Collider other){
        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory == null) return;

        int accepted = inventory.TryAdd(foodAmount);
        if(accepted>= foodAmount){
            Destroy(gameObject);
            OnCollected?.Invoke(this);
        }
        else{
            foodAmount -= accepted;
        }
    }
}
