using UnityEngine;

public class SchoolDebugLogger : MonoBehaviour
{
    [SerializeField] private School school;

    private void OnEnable()
    {
        school.OnDeliveryProgress += LogProgress;
        school.OnCompleted += LogCompleted;
    }

    private void OnDisable()
    {
        school.OnDeliveryProgress -= LogProgress;
        school.OnCompleted -= LogCompleted;
    }

    private void LogProgress(int delivered, int required)
    {
        Debug.Log(school.name + ": " + delivered + "/" + required);
    }

    private void LogCompleted(IObjective completed)
    {
        Debug.Log(school.name + " completed!");
    }
}
