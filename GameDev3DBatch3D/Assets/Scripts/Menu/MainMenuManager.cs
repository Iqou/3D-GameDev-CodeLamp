using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject creditsPanel; 

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

    public void GoToLevelSelect()
    {
   
        SceneManager.LoadScene("Level Select");
    }

    public void GoToGameplay()
    {
    
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }

        SceneManager.LoadScene("Gameplay");
    }

    public void OpenCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }

    
    public void CloseCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
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