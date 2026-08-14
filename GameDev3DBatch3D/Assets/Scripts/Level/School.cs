using System;
using UnityEngine;

public class School : MonoBehaviour, IObjective
{
    [SerializeField] private string objectiveName = "School";
    [SerializeField, Min(1)] private int foodRequired = 5;
    [SerializeField] private int foodPerDelivery = 1;
    [SerializeField] private float deliveryInterval = 0.5f;

    private int foodDelivered;
    private float timer;

    private PlayerInventory deliverer;

    public string ObjectiveName => objectiveName;
    public int FoodRequired => foodRequired;
    public int FoodPerDelivery => foodPerDelivery;
    public bool IsComplete => foodDelivered >= foodRequired;

    public event Action<int, int> OnDeliveryProgress;

    public event Action<IObjective> OnCompleted;

    private void Start()
    {
        if(LevelManager.Instance != null)
        {
            LevelManager.Instance.Register(this);
        }
        else
        {
            Debug.LogWarning("No LevelManager found in the scene. School objective will not be registered.");
        }
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.Unregister(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
        if (inventory == null) return;

        deliverer = inventory;
        timer = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory == deliverer)
        {
            deliverer = null;
        }

    }

    private void Update()
    {
        if(deliverer == null || IsComplete) return;

        timer -= Time.deltaTime;
        if(timer > 0f) return;

        timer = deliveryInterval;
        Deliver();
    }

    private void Deliver()
    {
        int stillNeeded = foodRequired - foodDelivered;
        int amountToAsk = Mathf.Min(stillNeeded, foodPerDelivery);

        int taken = deliverer.TryRemove(amountToAsk);
        if (taken <= 0) return;

        foodDelivered += taken;

        OnDeliveryProgress?.Invoke(foodDelivered, foodRequired);

        if (IsComplete)
        {
            OnCompleted?.Invoke(this);
        }
    }
}

