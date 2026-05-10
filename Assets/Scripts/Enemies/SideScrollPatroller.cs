using UnityEngine;

/// <summary>
/// Simple side-to-side movement for platformer enemies with edge detection.
/// Walks back and forth, turning around when reaching a platform edge (Goomba-style).
/// </summary>
public class SideScrollPatroller : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float edgeDetectionDistance = 0.5f;

    private Rigidbody2D rb;
    private float currentDirection = 1f; // 1 for right, -1 for left
    private Collider2D groundCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCollider = GetComponent<Collider2D>();

        if (rb == null)
        {
            Debug.LogError("SideScrollPatroller requires a Rigidbody2D component!");
        }
        if (groundCollider == null)
        {
            Debug.LogError("SideScrollPatroller requires a Collider2D component!");
        }
    }

    private void Update()
    {
        // Check for edge detection ahead
        if (!IsGroundAheadOfEdge())
        {
            TurnAround();
        }
    }

    private void FixedUpdate()
    {
        // Apply movement velocity while preserving vertical velocity
        rb.velocity = new Vector2(moveSpeed * currentDirection, rb.velocity.y);
    }

    /// <summary>
    /// Check if there is ground ahead of the entity at the current direction.
    /// Returns false if we're at an edge (no ground ahead), triggering a turn.
    /// </summary>
    private bool IsGroundAheadOfEdge()
    {
        // Get the collider bounds to find the edge point
        Bounds bounds = groundCollider.bounds;
        
        // Determine the forward edge of the collider based on current direction
        float edgeX = currentDirection > 0 ? bounds.max.x : bounds.min.x;
        float checkX = edgeX + (edgeDetectionDistance * currentDirection);
        float checkY = bounds.center.y;

        // Cast a ray downward from the edge point
        Vector2 rayStart = new Vector2(checkX, checkY);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, edgeDetectionDistance);

        // Ground exists if we hit something
        return hit.collider != null;
    }

    private void TurnAround()
    {
        currentDirection *= -1f;
    }
}
