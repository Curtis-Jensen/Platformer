using UnityEngine;

// Crawls along any surface, wrapping corners instead of reversing.
// Hit a wall ahead → climb up it. Run out of floor → descend the edge.
//
// surfaceNormal always points OUTWARD from the surface toward the worm (like a floor normal).
// Turn formulas (both cases):  new surfaceNormal = old moveDir
//   Wall hit:   new moveDir =  surfaceNormal   (turn up)
//   Edge wrap:  new moveDir = -surfaceNormal   (turn down)
[RequireComponent(typeof(BoxCollider2D))]
public class SurfaceHugger : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float probeDistance = 0.2f;  // how far raycasts reach past the body
    [SerializeField] private LayerMask groundLayer;

    private Vector2 moveDir       = Vector2.right;
    private Vector2 surfaceNormal = Vector2.up;

    private float turnCooldown;
    private BoxCollider2D col;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        turnCooldown -= Time.deltaTime;

        // Use local col.size so extents aren't corrupted by transform rotation
        Vector2 localHalf = col.size * 0.5f;
        float halfForward = localHalf.x;   // along the worm's body length
        float halfPerp    = localHalf.y;   // across the worm's body

        Vector2 center = (Vector2)transform.position + (Vector2)(transform.rotation * col.offset);

        // Wall probe: from the front face, cast forward
        Vector2 wallOrigin  = center + moveDir * halfForward;
        // Floor probe: from the front-bottom corner, cast toward the surface
        Vector2 floorOrigin = wallOrigin - surfaceNormal * halfPerp;

        bool wallAhead  = Physics2D.Raycast(wallOrigin,  moveDir,         probeDistance, groundLayer);
        bool floorAhead = Physics2D.Raycast(floorOrigin, -surfaceNormal,  probeDistance, groundLayer);

        if (turnCooldown <= 0f)
        {
            if (wallAhead)
            {
                Vector2 oldSN = surfaceNormal;
                surfaceNormal = moveDir;
                moveDir       = oldSN;
                turnCooldown  = 0.25f;
            }
            else if (!floorAhead)
            {
                // Scoot to the corner in the OLD moveDir before pivoting
                transform.position += (Vector3)(moveDir * (halfForward + 0.02f));
                Vector2 oldSN = surfaceNormal;
                surfaceNormal = moveDir;
                moveDir       = -oldSN;
                turnCooldown  = 0.25f;
            }
        }

        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);

        // Rotate sprite to lie flat on whatever surface we're hugging
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();
        Vector2 localHalf   = col.size * 0.5f;
        Vector2 center      = (Vector2)transform.position + (Vector2)(transform.rotation * col.offset);
        Vector2 wallOrigin  = center + moveDir * localHalf.x;
        Vector2 floorOrigin = wallOrigin - surfaceNormal * localHalf.y;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(wallOrigin,  moveDir        * probeDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(floorOrigin, -surfaceNormal * probeDistance);
    }
#endif
}
