using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject creditsPanel;

    [Header("Audio")]
    public string buttonSoundName = "Button";   // Nama sound yang dipanggil ke SoundManager
    public float buttonSoundDelay = 0.15f;      // Jeda sebelum pindah scene / keluar game

    private bool isTransitioning = false;       // Cegah tombol diklik dobel saat menunggu

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    // =====================================================
    //  HELPER SUARA & TRANSISI
    // =====================================================
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

    private IEnumerator LoadSceneAfterSound(string sceneName, bool playGameplayMusic = false)
    {
        // Realtime -> tetap jalan walau Time.timeScale = 0
        if (buttonSoundDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(buttonSoundDelay);
        }

        if (playGameplayMusic && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }

        Time.timeScale = 1f; // Pastikan scene berikutnya tidak macet
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator QuitAfterSound()
    {
        if (buttonSoundDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(buttonSoundDelay);
        }

        Debug.Log("Keluar dari Game...");
        Application.Quit();
    }

    // =====================================================
    //  FUNGSI UNTUK TOMBOL UI
    // =====================================================
    public void GoToLevelSelect()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        PlayButtonSound();
        StartCoroutine(LoadSceneAfterSound("Level Select"));
    }

    public void GoToGameplay()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        PlayButtonSound();
        StartCoroutine(LoadSceneAfterSound("Gameplay", true));
    }

    public void OpenCredits()
    {
        PlayButtonSound(); // Tidak perlu delay, tidak pindah scene

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }

    public void CloseCredits()
    {
        PlayButtonSound(); // Tidak perlu delay, tidak pindah scene

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        PlayButtonSound();
        StartCoroutine(LoadSceneAfterSound(sceneName));
    }

    public void QuitGame()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        PlayButtonSound();
        StartCoroutine(QuitAfterSound());
    }

    public void OnGameExitPress()
    {
        PlayButtonSound(); 
 
        SceneManager.LoadScene("Main Menu"); 
    }
}