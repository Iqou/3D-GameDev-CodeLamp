using TMPro;
using UnityEngine;

public class ObjectiveHUD : MonoBehaviour 
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private string header = "Deliveries";
    [SerializeField] private string allCompleteMessage = "balik ke tempat mula!";

    private LevelManager manager;
    private bool allComplete;

    private void Start()
    {
        manager = LevelManager.Instance;

        if (manager == null)
        {
            label.text = "";
            return;
        }

        manager.OnObjectiveRegistered += HandleObjectiveChanged;
        manager.OnObjectiveCompleted += HandleObjectiveChanged;
        manager.OnAllObjectivesCompleted += HandleAllComplete;

        Rebuild();
    }

    private void OnDestroy()
    {
        if (manager == null) return;

        manager.OnObjectiveRegistered -= HandleObjectiveChanged;
        manager.OnObjectiveCompleted -= HandleObjectiveChanged;
        manager.OnAllObjectivesCompleted -= HandleAllComplete;
    }

    private void HandleObjectiveChanged(IObjective objective)
    {
        Rebuild();
    }

    private void HandleAllComplete()
    {
        allComplete = true;
        Rebuild();
    }

    private void Rebuild()
    {
        if (allComplete)
        {
            label.text = allCompleteMessage;
            return;
        }

        string text = header + "\n";

        foreach (IObjective objective in manager.Objectives)
        {
            text += (objective.IsComplete ? "[x] " : "[ ] ") + objective.ObjectiveName + "\n";
        }

        label.text = text;
    }
}