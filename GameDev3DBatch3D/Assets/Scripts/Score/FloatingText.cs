using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public Vector3 vector3Offset = new Vector3(0, 2, 0); // Offset for the floating text position
    public float floatingtextduration = 1f; // Duration for which the floating text is visible

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        Destroy(gameObject, floatingtextduration);

        transform.position += vector3Offset; // Apply the offset to the position of the floating text
    }

    private void LateUpdate()
    {
        // Make the floating text face the camera
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
