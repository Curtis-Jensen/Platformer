using UnityEngine;

// Attach to the background sprite (child of the camera).
// Records whatever scale looks correct at startup, then scales
// proportionally as the camera zooms in or out.
public class BackgroundScaler : MonoBehaviour
{
    private Camera cam;
    private float baseOrthoSize;
    private Vector3 baseScale;

    private void Start()
    {
        cam = Camera.main;
        baseOrthoSize = cam.orthographicSize;
        baseScale = transform.localScale;
    }

    private void LateUpdate()
    {
        float ratio = cam.orthographicSize / baseOrthoSize;
        transform.localScale = baseScale * ratio;
    }
}
