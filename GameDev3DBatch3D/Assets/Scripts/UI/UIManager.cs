using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Wajib untuk UI TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseUI;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;

    [Header("Objective Table Panel")]
    public GameObject objectiveContent;
    public TMP_Text objectiveButtonText;
    private bool isObjectiveOpen = true;

    [Header("UI Health")]
    public Image healthBar;
    public float maxHealth = 3f;
    public float healthAmount = 3f;

    [Header("UI Timer (3 Menit)")]
    public TMP_Text timerText;
    public float timeRemaining = 180f; // 180 detik = 3 menit
    private bool timerIsRunning = false;

    private bool isGameOver = false;

    void Start()
    {
        // Pastikan waktu game berjalan normal
        Time.timeScale = 1f;

        // Mulai timer
        timerIsRunning = true;

        // Sembunyikan semua panel UI di awal
        if (pauseUI != null) pauseUI.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameWinPanel != null) gameWinPanel.SetActive(false);

        // Set teks awal pada tombol toggle objective
        UpdateObjectiveButtonText();
    }

    void Update()
    {
        // Jika permainan sudah selesai (Game Over atau Menang), hentikan semua kalkulasi
        if (isGameOver) return;

        // --- SISTEM TIMER ---
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                DisplayTime(0);
                GameOver(); // Game Over jika waktu habis
            }
        }

        // --- CEK DARAH HABIS ---
        if (healthAmount <= 0)
        {
            GameOver();
        }

        // --- INPUT DETEKSI ---
        // Tekan 'Q' untuk Menang / Kamu Berhasil
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameWin();
        }

        // Testing Input (Damage & Heal)
        if (Input.GetKeyDown(KeyCode.Backspace)) TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.Space)) Heal(1);
    }

    // --- FUNGSI TAMPILAN WAKTU (MM:SS) ---
    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // --- MANAJEMEN OBJECTIVE TABEL (TOGGLE MINIMIZE) ---
    public void ToggleObjectivePanel()
    {
        isObjectiveOpen = !isObjectiveOpen;

        if (objectiveContent != null)
        {
            objectiveContent.SetActive(isObjectiveOpen);
        }

        UpdateObjectiveButtonText();
    }

    void UpdateObjectiveButtonText()
    {
        if (objectiveButtonText != null)
        {
            objectiveButtonText.text = isObjectiveOpen ? "Tugas" : "Tugas";
        }
    }

    // --- MANAJEMEN DARAH ---
    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.fillAmount = healthAmount / maxHealth;
        }
    }

    public void Heal(float amount)
    {
        healthAmount += amount;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.fillAmount = healthAmount / maxHealth;
        }
    }

    // --- PAUSE, GAME OVER, & GAME WIN ---
    void GameOver()
    {
        isGameOver = true;
        timerIsRunning = false;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Hentikan pergerakan/waktu game
    }

    public void GameWin()
    {
        isGameOver = true;
        timerIsRunning = false;

        if (gameWinPanel != null) gameWinPanel.SetActive(true);
        Time.timeScale = 0f; // Hentikan game saat menang
    }

    public void OnEnterPausePress()
    {
        if (pauseUI != null) pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnGameResumePress()
    {
        if (pauseUI != null) pauseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnRestartPress()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- FUNGSI EXIT KEMBALI KE MAIN MENU ---
    public void OnGameExitPress()
    {
        Time.timeScale = 1f; // Resets kecepatan waktu agar game di Main Menu tidak macet
        SceneManager.LoadScene("Main Menu"); // Kembali ke scene MainMenu
    }
}