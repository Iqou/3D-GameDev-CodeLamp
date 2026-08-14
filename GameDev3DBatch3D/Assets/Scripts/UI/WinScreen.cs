using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private bool pausedOnWin= true;

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
        if(manager != null)
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

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
