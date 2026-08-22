using System.Collections;
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

    [Header("Objective Tablet Panel")]
    public RectTransform objectiveContentRect;  // Panel isi objective (yang keluar layar)
    public RectTransform objectiveButtonRect;   // Tombol toggle (tetap terlihat di layar)
    public TMP_Text objectiveButtonText;
    public CanvasGroup objectiveCanvasGroup;    // Opsional: untuk fade + blok klik saat tertutup

    [Header("Slide Settings")]
    public float contentSlideDistance = 360f;   // Seberapa jauh CONTENT turun (bikin keluar layar)
    public float buttonSlideDistance = 120f;    // Seberapa jauh TOMBOL turun (jangan sampai keluar layar)
    public bool autoUseContentHeight = false;   // Kalau true, jarak content dihitung dari tinggi panel
    public float autoExtraPadding = 50f;        // Tambahan jarak saat autoUseContentHeight aktif
    public float slideDuration = 0.35f;         // Durasi animasi (detik)
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool startOpened = true;             // Kondisi awal saat game mulai
    public bool fadeContentWhenHidden = false;  // Opsional: ikut memudar saat tertutup

    [Header("Audio")]
    public string buttonSoundName = "Button";   // Nama sound yang dipanggil ke SoundManager

    private Vector2 contentShownPos;
    private Vector2 buttonShownPos;
    private Coroutine slideRoutine;
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

        // Simpan posisi awal (posisi saat panel terbuka) lalu set kondisi awal
        CacheObjectivePositions();
        isObjectiveOpen = startOpened;
        ApplyObjectiveState(isObjectiveOpen, true); // true = instan, tanpa animasi

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameWin();
        }

        // Testing Input (Damage & Heal)
        if (Input.GetKeyDown(KeyCode.Backspace)) TakeDamage(1);
        
    }

    // =====================================================
    //  HELPER SUARA TOMBOL
    // =====================================================
    void PlayButtonSound()
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

    // =====================================================
    //  OBJECTIVE TABLET SLIDE (turun keluar layar / kembali)
    // =====================================================

    void CacheObjectivePositions()
    {
        if (objectiveContentRect != null)
        {
            contentShownPos = objectiveContentRect.anchoredPosition;

            // Hitung otomatis jarak turun berdasarkan tinggi panel
            if (autoUseContentHeight)
            {
                contentSlideDistance = objectiveContentRect.rect.height + autoExtraPadding;
            }
        }

        if (objectiveButtonRect != null)
        {
            buttonShownPos = objectiveButtonRect.anchoredPosition;
        }
    }

    // Dipasang di OnClick() tombol Tugas
    public void ToggleObjectivePanel()
    {

        isObjectiveOpen = !isObjectiveOpen;
        ApplyObjectiveState(isObjectiveOpen, false);
        UpdateObjectiveButtonText();
    }

    // Bonus: kalau butuh buka/tutup dari script lain (tanpa suara tombol)
    public void SetObjectivePanel(bool open)
    {
        if (isObjectiveOpen == open) return;
        isObjectiveOpen = open;
        ApplyObjectiveState(isObjectiveOpen, false);
        UpdateObjectiveButtonText();
    }

    void ApplyObjectiveState(bool open, bool instant)
    {
        // Saat tertutup, langsung matikan klik supaya tombol di dalam panel tidak bisa ditekan
        if (objectiveCanvasGroup != null)
        {
            objectiveCanvasGroup.interactable = open;
            objectiveCanvasGroup.blocksRaycasts = open;
        }

        if (slideRoutine != null)
        {
            StopCoroutine(slideRoutine);
            slideRoutine = null;
        }

        if (instant || slideDuration <= 0f)
        {
            if (objectiveContentRect != null)
                objectiveContentRect.anchoredPosition = GetContentTarget(open);

            if (objectiveButtonRect != null)
                objectiveButtonRect.anchoredPosition = GetButtonTarget(open);

            if (fadeContentWhenHidden && objectiveCanvasGroup != null)
                objectiveCanvasGroup.alpha = open ? 1f : 0f;

            return;
        }

        slideRoutine = StartCoroutine(SlideObjectiveRoutine(open));
    }

    Vector2 GetContentTarget(bool open)
    {
        return open ? contentShownPos : contentShownPos + Vector2.down * contentSlideDistance;
    }

    Vector2 GetButtonTarget(bool open)
    {
        return open ? buttonShownPos : buttonShownPos + Vector2.down * buttonSlideDistance;
    }

    IEnumerator SlideObjectiveRoutine(bool open)
    {
        Vector2 contentStart = objectiveContentRect != null ? objectiveContentRect.anchoredPosition : Vector2.zero;
        Vector2 buttonStart = objectiveButtonRect != null ? objectiveButtonRect.anchoredPosition : Vector2.zero;

        Vector2 contentTarget = GetContentTarget(open);
        Vector2 buttonTarget = GetButtonTarget(open);

        float startAlpha = objectiveCanvasGroup != null ? objectiveCanvasGroup.alpha : 1f;
        float targetAlpha = open ? 1f : 0f;

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            // unscaledDeltaTime -> animasi tetap jalan walau Time.timeScale = 0 (saat pause)
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / slideDuration);
            float eased = slideCurve.Evaluate(t);

            if (objectiveContentRect != null)
                objectiveContentRect.anchoredPosition = Vector2.LerpUnclamped(contentStart, contentTarget, eased);

            if (objectiveButtonRect != null)
                objectiveButtonRect.anchoredPosition = Vector2.LerpUnclamped(buttonStart, buttonTarget, eased);

            if (fadeContentWhenHidden && objectiveCanvasGroup != null)
                objectiveCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        // Snap ke posisi akhir supaya presisi
        if (objectiveContentRect != null) objectiveContentRect.anchoredPosition = contentTarget;
        if (objectiveButtonRect != null) objectiveButtonRect.anchoredPosition = buttonTarget;
        if (fadeContentWhenHidden && objectiveCanvasGroup != null) objectiveCanvasGroup.alpha = targetAlpha;

        slideRoutine = null;
    }

    void UpdateObjectiveButtonText()
    {
        if (objectiveButtonText != null)
        {
            objectiveButtonText.text = isObjectiveOpen ? "Tugas \u25BC" : "Tugas \u25B2";
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
        PlayButtonSound(); // <-- SUARA TOMBOL

        if (pauseUI != null) pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnGameResumePress()
    {
        PlayButtonSound(); // <-- SUARA TOMBOL

        if (pauseUI != null) pauseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnRestartPress()
    {
        PlayButtonSound(); // <-- SUARA TOMBOL

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- FUNGSI EXIT KEMBALI KE MAIN MENU ---
    public void OnGameExitPress()
    {
        PlayButtonSound(); // <-- SUARA TOMBOL

        Time.timeScale = 1f; // Reset kecepatan waktu agar game di Main Menu tidak macet
        SceneManager.LoadScene("Level Select"); // Kembali ke scene MainMenu
    }
}