using UnityEngine;

// Attach to the Camera. Finds the player by tag -- no parenting needed.
public class PlayerFollower : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string playerTag = "Player";

    [Header("Follow")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);

    [Header("Deadzone")]
    [Tooltip("Camera won't move until the player is this far from camera center")]
    [SerializeField] private Vector2 deadzone = new Vector2(1f, 0.5f);

    [Header("Lookahead")]
    [Tooltip("How far ahead of the player the camera peeks in the movement direction")]
    [SerializeField] private float lookaheadDistance = 1.5f;
    [Tooltip("How quickly the lookahead shifts (lower = lazier)")]
    [SerializeField] private float lookaheadSpeed = 3f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 12f;

    [Header("Bounds")]
    [Tooltip("Clamp the camera within these world-space bounds (leave at 0 to disable)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minX = -100f;
    [SerializeField] private float maxX = 100f;
    [SerializeField] private float minY = -100f;
    [SerializeField] private float maxY = 100f;

    private Transform target;
    private Camera cam;
    private float currentLookahead;
    private float lastTargetX;

    private void Start()
    {
        cam = GetComponent<Camera>();
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null)
        {
            target = player.transform;
            lastTargetX = target.position.x;
        }
    }

    // -------------------------------------------------------
    // HandleZoom()
    // Reads scroll wheel input and adjusts orthographic size,
    // clamped between minZoom and maxZoom.
    // -------------------------------------------------------
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.001f) return;
        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize - scroll * zoomSpeed,
            minZoom,
            maxZoom
        );
    }

    private void LateUpdate()
    {
        if (target == null) return;

        HandleZoom();

        // Lookahead: drift toward where the player is heading
        float moveDir = target.position.x - lastTargetX;
        if (Mathf.Abs(moveDir) > 0.01f)
            currentLookahead = Mathf.Lerp(currentLookahead, Mathf.Sign(moveDir) * lookaheadDistance, lookaheadSpeed * Time.deltaTime);
        lastTargetX = target.position.x;

        Vector3 desired = new Vector3(
            target.position.x + offset.x + currentLookahead,
            target.position.y + offset.y,
            transform.position.z
        );

        // Deadzone: only move if outside the deadzone box
        float dx = desired.x - transform.position.x;
        float dy = desired.y - transform.position.y;
        Vector3 goal = transform.position;
        if (Mathf.Abs(dx) > deadzone.x) goal.x = desired.x - Mathf.Sign(dx) * deadzone.x;
        if (Mathf.Abs(dy) > deadzone.y) goal.y = desired.y - Mathf.Sign(dy) * deadzone.y;

        Vector3 smoothed = Vector3.Lerp(transform.position, goal, smoothSpeed * Time.deltaTime);

        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX);
            smoothed.y = Mathf.Clamp(smoothed.y, minY, maxY);
        }

        transform.position = smoothed;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw the deadzone box in the Scene view
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(deadzone.x * 2f, deadzone.y * 2f, 0f));

        if (useBounds)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
#endif
}
