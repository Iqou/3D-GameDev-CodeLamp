using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Round Settings")]
    [SerializeField] private float roundDuration = 240f;

    [Header("Runtime")]
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool roundFinished;

    // Public read-only access
    public float TimeRemaining => timeRemaining;
    public bool RoundFinished => roundFinished;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        timeRemaining = roundDuration;
        roundFinished = false;
    }

    private void Update()
    {
        if (roundFinished)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            roundFinished = true;

            Debug.Log("Time's Up!");
            // TODO: Call your Game Over / End Round function here
        }
    }

    public void ResetTimer()
    {
        timeRemaining = roundDuration;
        roundFinished = false;
    }
}