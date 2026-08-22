using System;
using System.Collections;
using UnityEngine;

public class ObjectiveSpotlight : MonoBehaviour
{
    [SerializeField] private CutsceneCamera cutscene;

    [Header("Kapan cutscene diputar")]
    [SerializeField] private bool playOnLevelStart = true;
    [SerializeField] private bool playOnObjectiveCompleted = true;
    [SerializeField] private bool allowManualReplay = true;
    [SerializeField] private KeyCode replayKey = KeyCode.Q;

    [Header("Tujuan terakhir")]
    [SerializeField] private Transform finishFocusPoint;
    [SerializeField] private string finishHint = "balik ke tempat mula!";

    private LevelManager manager;

    public event Action<string> OnHintShown;
    public event Action OnHintHidden;

    private void Start()
    {
        manager = LevelManager.Instance;
        if (manager == null || cutscene == null) return;

        if (playOnObjectiveCompleted) manager.OnObjectiveCompleted += HandleObjectiveCompleted;
        manager.OnAllObjectivesCompleted += HandleAllComplete;
        cutscene.OnCutsceneFinished += HandleCutsceneFinished;

        if (playOnLevelStart) StartCoroutine(ShowAfterRegistration());
    }

    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.OnObjectiveCompleted -= HandleObjectiveCompleted;
            manager.OnAllObjectivesCompleted -= HandleAllComplete;
        }

        if (cutscene != null) cutscene.OnCutsceneFinished -= HandleCutsceneFinished;
    }

        private void Update()
    {
        if (manager == null || cutscene == null) return;                      // NEW

        if (allowManualReplay && Input.GetKeyDown(replayKey)) ShowNextObjective();
    }

    private IEnumerator ShowAfterRegistration()
    {
        yield return null;      // tunggu 1 frame: semua School sudah Register() di Start
        ShowNextObjective();
    }

    private void HandleObjectiveCompleted(IObjective objective)
    {
        ShowNextObjective();
    }

    private void HandleAllComplete()
    {
        if (finishFocusPoint != null) Show(finishFocusPoint, finishHint);
    }

    // Cari objective pertama yang belum selesai DAN punya titik kamera.
    public void ShowNextObjective()
    {
        IObjectiveFocus best = null;
        IObjective bestObjective = null;

        foreach (IObjective objective in manager.Objectives)
        {
            if (objective.IsComplete) continue;

            IObjectiveFocus focus = objective as IObjectiveFocus;

            if (focus == null)
            {
                Debug.LogWarning("Spotlight: " + objective.ObjectiveName
                    + " -> tidak mengimplementasikan IObjectiveFocus", objective as MonoBehaviour);
                continue;
            }

            if (focus.FocusPoint == null)
            {
                Debug.LogWarning("Spotlight: " + objective.ObjectiveName
                    + " -> Focus Point kosong", objective as MonoBehaviour);
                continue;
            }

            // CHANGED: bukan "yang pertama ketemu", tapi order paling kecil
            if (best == null || focus.SpotlightOrder < best.SpotlightOrder)
            {
                best = focus;
                bestObjective = objective;
            }
        }

        if (best != null)
        {
            Debug.Log("Spotlight: menunjukkan " + bestObjective.ObjectiveName
                + " (order " + best.SpotlightOrder + ")");
            Show(best.FocusPoint, best.HintText);
            return;
        }

        bool semuaSelesai = manager.TotalCount > 0 && manager.CompletedCount >= manager.TotalCount;

        if (semuaSelesai && finishFocusPoint != null)
        {
            Show(finishFocusPoint, finishHint);
            return;
        }

        Debug.LogWarning("Spotlight: tidak ada yang bisa ditunjukkan.", this);
    }

    private void Show(Transform point, string hint)
    {
        cutscene.Play(point);

        if (cutscene.IsPlaying) OnHintShown?.Invoke(hint);
    }

    private void HandleCutsceneFinished()
    {
        OnHintHidden?.Invoke();
    }
}