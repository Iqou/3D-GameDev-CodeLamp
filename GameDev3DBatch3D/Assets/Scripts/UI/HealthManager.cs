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

    [Header("Damage dari Obstacle")]
    public string obstacleTag = "Obstacle"; // Tag object yang bikin luka
    public float damagePerHit = 1f;         // Berkurang 1 darah per tabrakan
    public float invincibleTime = 0.3f;     // Jeda kebal antar hit (jaga-jaga collider ganda)

    [Header("Hancurkan Obstacle")]
    public bool destroyObstacle = true;     // Obstacle hilang setelah disentuh
    public float destroyDelay = 0f;         // 0 = langsung hilang
    public GameObject destroyEffect;        // Opsional: prefab partikel/ledakan

    private bool isGameOver = false;
    private float lastHitTime = -999f;

    void Start()
    {
        // Pastikan panel Game Over tersembunyi saat game mulai
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Memastikan waktu berjalan normal saat game mulai
        Time.timeScale = 1f;

        UpdateHealthBar();
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
            TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Heal(1);
        }
    }

    // ===== DETEKSI TABRAKAN 2D =====
    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleObstacleTouch(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleObstacleTouch(other.gameObject);
    }

    // ===== DETEKSI TABRAKAN 3D (kalau project-mu 3D) =====
    void OnCollisionEnter(Collision collision)
    {
        HandleObstacleTouch(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleObstacleTouch(other.gameObject);
    }

    // Inti logika: cek tag -> kurangi darah -> hancurkan obstacle
    private void HandleObstacleTouch(GameObject hitObject)
    {
        if (isGameOver || hitObject == null) return;

        GameObject obstacle = GetObstacleRoot(hitObject);
        if (obstacle == null) return;

        // Masih dalam masa kebal? abaikan
        if (Time.time - lastHitTime < invincibleTime) return;
        lastHitTime = Time.time;

        TakeDamage(damagePerHit);

        if (destroyObstacle)
        {
            DestroyObstacle(obstacle);
        }
    }

    // Cari object ber-tag Obstacle (termasuk kalau collider-nya ada di child)
    private GameObject GetObstacleRoot(GameObject hitObject)
    {
        if (hitObject.CompareTag(obstacleTag)) return hitObject;

        Transform parent = hitObject.transform.parent;
        while (parent != null)
        {
            if (parent.CompareTag(obstacleTag)) return parent.gameObject;
            parent = parent.parent;
        }

        return null;
    }

    private void DestroyObstacle(GameObject obstacle)
    {
        // Munculkan efek hancur (opsional)
        if (destroyEffect != null)
        {
            GameObject fx = Instantiate(destroyEffect, obstacle.transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // Matikan collider dulu supaya tidak memicu damage kedua
        Collider2D[] colliders2D = obstacle.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders2D) col.enabled = false;

        Collider[] colliders3D = obstacle.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders3D) col.enabled = false;

        if (destroyDelay <= 0f)
        {
            Destroy(obstacle); // Langsung hilang
        }
        else
        {
            Destroy(obstacle, destroyDelay); // Hilang setelah beberapa detik
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);

        UpdateHealthBar();

        Debug.Log("-" + damage + " health | sisa: " + healthAmount);

        if (healthAmount <= 0)
        {
            GameOver();
        }
    }

    public void Heal(float amount)
    {
        healthAmount += amount;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = healthAmount / maxHealth;
        }
    }

    void GameOver()
    {
        if (isGameOver) return;

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
    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Keluar dari Game...");
        Application.Quit();
    }
}