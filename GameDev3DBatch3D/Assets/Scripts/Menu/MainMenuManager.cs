using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        // Panggil BGM Menu saat scene dibuka
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }
    }

    public void GoToLevelSelect()
    {
        // Pindah ke Level Select (BGM tetap lanjut seamless karena lagunya sama)
        SceneManager.LoadScene("Level Select");
    }

    public void GoToGameplay()
    {
        // Pindah BGM ke BGM Gameplay sebelum/saat pindah scene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }

        SceneManager.LoadScene("Gameplay");
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Keluar dari Game...");
        Application.Quit();
    }
}