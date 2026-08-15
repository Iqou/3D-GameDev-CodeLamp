using UnityEngine;

// Pengaturan spin yang bisa dipakai bareng oleh spawner dan item-nya
[System.Serializable]
public class SpinSettings
{
    [Header("Spin")]
    public bool spinEnabled = true;
    public float spinSpeed = 90f;                 // Derajat per detik (90 = 1 putaran / 4 detik)
    public Vector3 spinAxis = Vector3.up;         // 3D: (0,1,0) | Sprite 2D: (0,0,1)
    public bool spinInWorldSpace = true;
    public bool randomizeStartRotation = false;   // Biar tiap crate tidak menghadap arah yang sama

    [Header("Bobbing (naik-turun, opsional)")]
    public bool bobEnabled = false;
    public float bobAmplitude = 0.15f;            // Seberapa tinggi gerakan naik-turun
    public float bobSpeed = 2f;                   // Kecepatan naik-turun
    public bool randomizeBobPhase = true;         // Biar tidak naik-turun serempak
}

public class ItemSpinner : MonoBehaviour
{
    [SerializeField] private SpinSettings settings = new SpinSettings();

    private Vector3 startLocalPos;
    private float bobPhase;

    private void Awake()
    {
        // Posisi awal disimpan supaya bobbing tidak "hanyut" naik terus
        startLocalPos = transform.localPosition;
    }

    private void Start()
    {
        bobPhase = settings.randomizeBobPhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;

        if (settings.randomizeStartRotation && settings.spinAxis.sqrMagnitude > 0.0001f)
        {
            transform.Rotate(
                settings.spinAxis.normalized,
                Random.Range(0f, 360f),
                settings.spinInWorldSpace ? Space.World : Space.Self);
        }
    }

    private void Update()
    {
        // --- SPIN DI TEMPAT ---
        if (settings.spinEnabled && settings.spinAxis.sqrMagnitude > 0.0001f)
        {
            transform.Rotate(
                settings.spinAxis.normalized * settings.spinSpeed * Time.deltaTime,
                settings.spinInWorldSpace ? Space.World : Space.Self);
        }

        // --- BOBBING NAIK-TURUN ---
        if (settings.bobEnabled)
        {
            float offsetY = Mathf.Sin(Time.time * settings.bobSpeed + bobPhase) * settings.bobAmplitude;
            transform.localPosition = startLocalPos + Vector3.up * offsetY;
        }
    }

    // Dipakai spawner untuk menimpa pengaturan saat crate dibuat
    public void ApplySettings(SpinSettings newSettings)
    {
        if (newSettings != null)
        {
            settings = newSettings;
        }
    }
}