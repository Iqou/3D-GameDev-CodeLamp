using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private readonly List<IObjective> objectives = new List<IObjective>();
    private int completedCount;

    public IReadOnlyList<IObjective> Objectives => objectives;
    public int TotalCount => objectives.Count;
    public int CompletedCount => completedCount;

    public bool AllObjectivesCompleted => objectives.Count > 0 && completedCount >= objectives.Count;
    public bool LevelWon { get; private set; }

    public event Action<IObjective> OnObjectiveRegistered;
    public event Action<IObjective> OnObjectiveCompleted;
    public event Action OnAllObjectivesCompleted;
    public event Action OnLevelWon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Register(IObjective objective)
    {
        if (objective == null || objectives.Contains(objective)) return;

        objectives.Add(objective);
        objective.OnCompleted += HandleObjectiveCompleted;

        OnObjectiveRegistered?.Invoke(objective);
        Debug.Log($"Objective registered: {objective.ObjectiveName}. Total objectives: {objectives.Count}");
    }

    public void Unregister(IObjective objective)
    {
        if (objective == null || objectives.Remove(objective)) return;

        objective.OnCompleted -= HandleObjectiveCompleted;
        if(objective.IsComplete)
        {
            completedCount--;
        }
    }

    private void HandleObjectiveCompleted(IObjective objective)
    {
        completedCount++;

        OnObjectiveCompleted?.Invoke(objective);
        Debug.Log($"Objective completed: {objective.ObjectiveName}. Completed objectives: {completedCount}/{objectives.Count}");

        if (AllObjectivesCompleted)
        {
            Debug.Log("All objectives completed!");
            OnAllObjectivesCompleted?.Invoke();
        }
    }

    public void WinLevel()
    {
        if (LevelWon || !AllObjectivesCompleted) return;

        LevelWon = true;
        Debug.Log("Level Won!");
        OnLevelWon?.Invoke();
    }
}
