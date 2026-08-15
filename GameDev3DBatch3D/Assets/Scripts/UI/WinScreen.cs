using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private bool pausedOnWin = true;
    [SerializeField] private string buttonSoundName = "Button"; // Nama sound tombol

    private LevelManager manager;

    private void Start()
    {
        panel.SetActive(false);

        manager = LevelManager.Instance;

        if (manager == null)
        {
            Debug.LogWarning("No LevelManager found in the scene. WinScreen will not be able to detect level completion.");
            return;
        }

        manager.OnLevelWon += Show;

        if (manager.LevelWon) Show();
    }

    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.OnLevelWon -= Show;
        }
    }

    private void Show()
    {
        panel.SetActive(true);

        if (pausedOnWin)
        {
            Time.timeScale = 0f;
        }
    }

    // --- HELPER SUARA TOMBOL ---
    private void PlayButtonSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound2D(buttonSoundName);
        }
        else
        {
            Debug.LogWarning("SoundManager.Instance tidak ditemukan di scene. Suara tombol dilewati.");
        }
    }

    // --- FUNGSI UNTUK TOMBOL UI ---
    public void RestartLevel()
    {
        PlayButtonSound(); // <-- SUARA TOMBOL

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Tombol tambahan (opsional) - pasang di OnClick() kalau dibutuhkan
    public void GoToMainMenu()
    {
        PlayButtonSound(); // <-- SUARA TOMBOL

        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        PlayButtonSound(); // <-- SUARA TOMBOL

        Debug.Log("Keluar dari Game...");
        Application.Quit();
    }
}