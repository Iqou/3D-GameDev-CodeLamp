using UnityEngine;

public class CarUnstuck : MonoBehaviour
{
    [Header("Stuck Detection Settings")]
    [SerializeField] private float maxTiltAngle = 70f;
    [SerializeField] private float checkInterval = 0.2f;
    [SerializeField] private float stuckTime = 1.0f;

    [Header("Recovery settings")]
    [SerializeField] private float liftAmount = 1.0f;
    [SerializeField] private bool zeroVelocity = true;

    private Rigidbody carRb;
    private float timer;
    private float tippedDuration;

    private void Awake()
    {
        carRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer < checkInterval) return;
        timer = 0f;

        float tilt = Vector3.Angle(transform.up, Vector3.up);
        bool isTipped = tilt > maxTiltAngle;

        if (isTipped)
        {
            tippedDuration += checkInterval;
            if (tippedDuration >= stuckTime)
            {
                ResetUpright();
                tippedDuration = 0f;
            }
        }
        else
        {
            tippedDuration = 0f;
        }
    }

    private void ResetUpright()
    {
        // Lift the car slightly to avoid ground collision
        Vector3 newPosition = transform.position + Vector3.up * liftAmount;
        carRb.MovePosition(newPosition);

        // Reset rotation to upright
        Quaternion uprightRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        carRb.MoveRotation(uprightRotation);

        // Optionally reset velocity
        if (zeroVelocity)
        {
            carRb.linearVelocity = Vector3.zero;
            carRb.angularVelocity = Vector3.zero;
        }
        carRb.Sleep(); 
        carRb.WakeUp();
    }
}
