using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int MaximumScore = 10000;
    public int CurrentScore;

    public int TimePenalty = 30;

    private float timer;

    [Header("Star Thresholds")]
    [SerializeField] private float threeStarThreshold = 0.75f;
    [SerializeField] private float twoStarThreshold = 0.50f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        CurrentScore = MaximumScore;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer -= 1f;

            RemoveScore(TimePenalty);
        }
    }

    public void RemoveScore(int amount)
    {
        CurrentScore -= amount;

        if (CurrentScore < 0)
            CurrentScore = 0;
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;

        if (CurrentScore > MaximumScore)
            CurrentScore = MaximumScore;
    }

    public int CalculateStars()
{
    float percentage = (float)CurrentScore / MaximumScore;

    if (percentage >= threeStarThreshold)
        return 3;

    if (percentage >= twoStarThreshold)
        return 2;

    return 1;
}
}