using UnityEngine;

public enum PlatformLoopMode { Loop, PingPong }

// The platform's scene position is always waypoint 0.
// Add additional world-space stops in the Inspector array.
// Pauses at each stop if pauseTime > 0.
[RequireComponent(typeof(Rigidbody2D))]
public class PlatformMover : MonoBehaviour
{
    [SerializeField] private Vector2[] additionalWaypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float pauseTime = 0f;
    [SerializeField] private PlatformLoopMode loopMode = PlatformLoopMode.PingPong;

    private Rigidbody2D rb;
    private Vector2[] waypoints;
    private int targetIndex;
    private int direction = 1;
    private float pauseTimer;

    // -------------------------------------------------------
    // Awake()
    // Builds the full waypoint list by prepending the platform's
    // current scene position to whatever was set in the Inspector.
    // -------------------------------------------------------
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        waypoints = new Vector2[1 + (additionalWaypoints?.Length ?? 0)];
        waypoints[0] = rb.position;
        if (additionalWaypoints != null)
            additionalWaypoints.CopyTo(waypoints, 1);

        targetIndex = waypoints.Length > 1 ? 1 : 0;
    }

    // -------------------------------------------------------
    // FixedUpdate()
    // Moves the platform toward the current target waypoint
    // via MovePosition (kinematic-safe, carries standing objects).
    // On arrival, advances the index according to loopMode:
    //   Loop     — wraps back to 0 after the last stop.
    //   PingPong — reverses direction at each end.
    // -------------------------------------------------------
    private void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 target = waypoints[targetIndex];
        Vector2 next = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if (Vector2.Distance(rb.position, target) < 0.01f)
        {
            AdvanceWaypoint();
            pauseTimer = pauseTime;
        }
    }

    // -------------------------------------------------------
    // AdvanceWaypoint()
    // Updates targetIndex based on the current loopMode.
    // -------------------------------------------------------
    private void AdvanceWaypoint()
    {
        if (loopMode == PlatformLoopMode.Loop)
        {
            targetIndex = (targetIndex + 1) % waypoints.Length;
        }
        else
        {
            targetIndex += direction;
            if (targetIndex >= waypoints.Length || targetIndex < 0)
            {
                direction = -direction;
                targetIndex += direction * 2;
            }
        }
    }
}
