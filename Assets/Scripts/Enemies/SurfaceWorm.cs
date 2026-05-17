using UnityEngine;

// Worm enemy that crawls along any surface, turning corners instead of reversing.
// Hit a wall ahead  → turn to climb it (rotate 90° toward the wall)
// No floor ahead    → turn to descend the edge (rotate 90° away from floor)
// Requires a single BoxCollider2D sized to the worm's body.
[RequireComponent(typeof(BoxCollider2D))]
public class SurfaceWorm : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rayLength = 0.15f;   // how far ahead / below to probe
    [SerializeField] private LayerMask groundLayer;

    // Travel direction in local space: right = forward along current surface.
    private int travelSign = 1; // +1 or -1

    private BoxCollider2D col;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        Vector2 forward = transform.right * travelSign;
        Vector2 down    = -transform.up;

        Vector2 size   = col.size;
        Vector2 center = (Vector2)transform.position + col.offset;

        // Front edge of the collider in world space
        Vector2 frontEdge = center + forward * (size.x * 0.5f);
        // Bottom-front corner (where floor ends / wall begins)
        Vector2 bottomFront = frontEdge + down * (size.y * 0.5f);

        bool wallAhead = Physics2D.Raycast(frontEdge,   forward, rayLength, groundLayer);
        bool floorAhead = Physics2D.Raycast(bottomFront, down,   rayLength, groundLayer);

        if (wallAhead)
        {
            // Rotate 90° away from the wall (climb up the face of it)
            // The new surface normal is opposite to forward, so we turn "up" relative to current facing
            transform.Rotate(0f, 0f, 90f * travelSign);
            // Step slightly away from the corner so we don't clip
            transform.position += (Vector3)(transform.up * (size.y * 0.5f + 0.01f));
        }
        else if (!floorAhead)
        {
            // No floor ahead: wrap down around the edge
            // Move to the corner first, then rotate 90° inward
            transform.position += (Vector3)(forward * (size.x * 0.5f + 0.01f));
            transform.Rotate(0f, 0f, -90f * travelSign);
        }
        else
        {
            // Normal travel along current surface
            transform.position += (Vector3)(forward * moveSpeed * Time.deltaTime);
        }

        // Keep the sprite facing the right direction for DirectionFlipper
        // (it reads world X movement, which naturally handles the rotation)
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();
        Vector2 forward = transform.right * travelSign;
        Vector2 down    = -transform.up;
        Vector2 size    = col.size;
        Vector2 center  = (Vector2)transform.position + col.offset;
        Vector2 frontEdge    = center + forward * (size.x * 0.5f);
        Vector2 bottomFront  = frontEdge + down * (size.y * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(frontEdge,   forward * rayLength);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(bottomFront, down    * rayLength);
    }
#endif
}
