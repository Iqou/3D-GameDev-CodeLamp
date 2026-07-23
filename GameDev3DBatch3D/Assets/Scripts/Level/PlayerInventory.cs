using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    [SerializeField] public int MaxCapacity = 5; 
    private int currentFood = 0;

    public event Action OnChanged;

    public int Count => currentFood;
    public int Max => MaxCapacity;
    public bool IsFull => currentFood >= MaxCapacity;

    public int TryAdd(int amount)
    {
        int freeSpace = MaxCapacity - currentFood;
        int acceptedAmount = Mathf.Min(amount, freeSpace);

        if (acceptedAmount > 0)
        {
            currentFood += acceptedAmount;
            OnChanged?.Invoke();
        }

        return acceptedAmount;
    }   

    public int TryRemove(int amount)
    {
        int removedAmount = Mathf.Min(amount, currentFood);

        if (removedAmount > 0)
        {
            currentFood -= removedAmount;
            OnChanged?.Invoke();
        }

        return removedAmount;
    }
}
