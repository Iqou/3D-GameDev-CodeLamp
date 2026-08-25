using TMPro;
using UnityEngine;


public class PickupPrompt : MonoBehaviour
{
    [SerializeField] private Transform visual;               // wadah teks
    [SerializeField] private TextMeshPro label;              // TMP 3D (bukan UGUI)
    [SerializeField] private string message = "Ambil makanan!";
    [SerializeField] private float visibleDistance = 20f;    // 0 = selalu tampil

    private Camera viewCamera;
    private float refreshTimer;

    private void Start()
    {
        if (visual == null || label == null)
        {
            Debug.LogWarning("PickupPrompt: Visual atau Label belum diisi.", this);
            enabled = false;
            return;
        }

        label.text = message;
    }

    private void LateUpdate()
    {
        Camera cam = GetViewCamera();
        if (cam == null) return;

        bool shouldShow = visibleDistance <= 0f
            || Vector3.Distance(cam.transform.position, transform.position) <= visibleDistance;

        if (visual.gameObject.activeSelf != shouldShow) visual.gameObject.SetActive(shouldShow);
        if (!shouldShow) return;

        visual.rotation = cam.transform.rotation;      // billboard
    }
    private Camera GetViewCamera()
    {
        refreshTimer -= Time.unscaledDeltaTime;

        if (viewCamera != null && viewCamera.isActiveAndEnabled && refreshTimer > 0f) return viewCamera;

        refreshTimer = 0.25f;
        Camera best = null;

        foreach (Camera cam in Camera.allCameras)
        {
            if (best == null || cam.depth > best.depth) best = cam;
        }

        viewCamera = best;
        return viewCamera;
    }
}