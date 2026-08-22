using TMPro;
using UnityEngine;

public class CutsceneHintUI : MonoBehaviour
{
    [SerializeField] private ObjectiveSpotlight spotlight;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI label;

    private void OnEnable()
    {
        if (spotlight == null || panel == null || label == null)
        {
            Debug.LogWarning("CutsceneHintUI: ada field yang belum diisi di Inspector.", this);
            return;
        }

        panel.SetActive(false);          // pastikan tersembunyi saat mulai

        spotlight.OnHintShown += Show;
        spotlight.OnHintHidden += Hide;
    }

    private void OnDisable()
    {
        if (spotlight == null) return;

        spotlight.OnHintShown -= Show;
        spotlight.OnHintHidden -= Hide;
    }

    private void Show(string hint)
    {
        label.text = hint;
        panel.SetActive(true);
    }

    private void Hide()
    {
        panel.SetActive(false);
    }
}