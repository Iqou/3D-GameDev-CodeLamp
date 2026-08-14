using UnityEngine;
using TMPro;

public class SchoolProgressLabel : MonoBehaviour
{
    [SerializeField] private School school;
    [SerializeField] private TextMeshProUGUI label;

    private void OnEnable()
    {
        school.OnDeliveryProgress += Refresh;
        school.OnCompleted += HandleCompleted;

        Refresh(school.FoodDelivered, school.FoodRequired);
    }

    private void OnDisable()
    {
        school.OnDeliveryProgress -= Refresh;
        school.OnCompleted -= HandleCompleted;
    }

    private void Refresh(int delivered, int required)
    {
        label.text = delivered + " / " + required;
    }

    private void HandleCompleted(IObjective objective)
    {
        label.text = "DONE";
    }
}
