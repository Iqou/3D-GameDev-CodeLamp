using System;
using System.Collections;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    [SerializeField] private Camera cutsceneCamera;
    [SerializeField] private float dollyDistance = 6f;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float holdDuration = 1f;

    [Header("Skip")]
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;   // CHANGED: bukan "tombol apa pun"
    [SerializeField] private float skipDelay = 0.4f;            // NEW: skip baru aktif setelah ini

    [SerializeField] private bool pauseGameplay = false;

    public bool IsPlaying { get; private set; }

    public event Action OnCutsceneStarted;
    public event Action OnCutsceneFinished;

    private float playedTime;                                   // NEW

    private void Awake()
    {
        if (cutsceneCamera != null) cutsceneCamera.enabled = false;
    }

    public void Play(Transform focusPoint)
    {
        if (IsPlaying || focusPoint == null || cutsceneCamera == null) return;

        StartCoroutine(PlayRoutine(focusPoint));
    }

    // NEW: satu tempat untuk semua syarat skip
    private bool SkipRequested()
    {
        return allowSkip && playedTime > skipDelay && Input.GetKeyDown(skipKey);
    }

    private IEnumerator PlayRoutine(Transform focusPoint)
    {
        IsPlaying = true;
        OnCutsceneStarted?.Invoke();

        if (pauseGameplay) Time.timeScale = 0f;

        Vector3 endPos = focusPoint.position;
        Quaternion rot = focusPoint.rotation;
        Vector3 startPos = endPos - focusPoint.forward * dollyDistance;

        cutsceneCamera.transform.SetPositionAndRotation(startPos, rot);
        cutsceneCamera.enabled = true;

        playedTime = 0f;
        yield return null;      // NEW: buang frame pemicu, biar tombol Q tidak ikut ke-skip

        float t = 0f;
        while (t < 1f)
        {
            if (SkipRequested()) break;

            playedTime += Time.unscaledDeltaTime;
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, moveDuration);
            cutsceneCamera.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        cutsceneCamera.transform.SetPositionAndRotation(endPos, rot);

        float hold = 0f;
        while (hold < holdDuration)
        {
            if (SkipRequested()) break;

            playedTime += Time.unscaledDeltaTime;
            hold += Time.unscaledDeltaTime;
            yield return null;
        }

        cutsceneCamera.enabled = false;
        if (pauseGameplay) Time.timeScale = 1f;

        IsPlaying = false;
        OnCutsceneFinished?.Invoke();
    }
}