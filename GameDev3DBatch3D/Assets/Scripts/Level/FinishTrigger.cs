using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask acceptedLayers;
    [SerializeField] private GameObject lockedVisual;
    [SerializeField] private GameObject unlockedVisual;

    private bool isArmed;
    private int occupantCount;

    private void Start()
    {
        if (acceptedLayers.value == 0)
        {
            Debug.LogWarning("FinishTrigger has no accepted layers set. It will not respond to any objects.");
        }

        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("No LevelManager found in the scene. FinishTrigger will not be able to check for level completion.");
            SetArmed(false);
            return;
        }

        LevelManager.Instance.OnAllObjectivesCompleted += HandleAllObjectivesCompleted;

        SetArmed(LevelManager.Instance.AllObjectivesCompleted);
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnAllObjectivesCompleted -= HandleAllObjectivesCompleted;
        }
    }

    private void HandleAllObjectivesCompleted()
    {
        SetArmed(true);
    }

    private void SetArmed(bool armed)
    {
        isArmed = armed;

        if (lockedVisual != null) lockedVisual.SetActive(!armed);
        if (unlockedVisual != null) unlockedVisual.SetActive(armed);

        TryWin();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAccepted(other)) return;

        occupantCount++;
        TryWin();
    }

    private void OnTriggerExit(Collider other)
    {
        if(!isAccepted(other)) return;

        occupantCount = Mathf.Max(0, occupantCount - 1);
    }

    private void TryWin()
    {
        if (!isArmed || occupantCount <= 0) return;

        LevelManager.Instance.WinLevel();
    }

    private bool isAccepted(Collider other)
    {
        return (acceptedLayers.value & (1 << other.gameObject.layer)) != 0;
    }
}
