using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Dibutuhkan untuk reload scene/pindah scene

public class HealthManager : MonoBehaviour
{
    [Header("UI Health")]
    public Image healthBar;
    public float maxHealth = 3f;
    public float healthAmount = 3f;

    [Header("Game Over UI")]
    public GameObject gameOverPanel; // Drag GameObject Panel Game Over di Inspector

    private bool isGameOver = false;

    void Start()
    {
        // Pastikan panel Game Over tersembunyi saat game mulai
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Memastikan waktu berjalan normal saat game mulai
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Jika sudah Game Over, hentikan pemicu damage/heal dari input
        if (isGameOver) return;

        // Cek jika darah habis
        if (healthAmount <= 0)
        {
            GameOver();
        }

        // Input untuk testing
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Heal(1);
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.fillAmount = healthAmount / maxHealth;
        }

        Debug.Log("-1 health");
    }

    public void Heal(float damage)
    {
        healthAmount += damage; // Diperbaiki dari healthAmount += healthAmount
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.fillAmount = healthAmount / maxHealth;
        }
    }

    void GameOver()
    {
        isGameOver = true;

        // Munculkan panel Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Hentikan pergerakan/waktu game (Opsional)
        Time.timeScale = 0f;
    }

    // --- FUNGSI UNTUK TOMBOL UI ---

    // Pasang fungsi ini pada OnClick() tombol Retry
    public void RetryGame()
    {
        Time.timeScale = 1f; // Kembalikan kecepatan waktu ke normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload scene saat ini
    }

    // Pasang fungsi ini pada OnClick() tombol Quit
    public void QuitGame()
    {
        Debug.Log("Keluar dari Game...");
        Application.Quit(); // Keluar dari game (hanya berfungsi di build exe/apk, bukan di editor)
    }
}