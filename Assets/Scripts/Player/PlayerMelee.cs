using System.Collections;
using UnityEngine;

// Hollow Knight-style melee attack for the player.
// Press K to swing. Direction is determined by held input:
//   - Hold Up    → upward swing
//   - Hold Down  → downward swing (useful mid-air)
//   - Otherwise  → left or right based on which way the player is facing
//
// Uses PlayerActions to gate the attack:
//   - Attack is locked for the full cooldown window (prevents double-swing)
//   - Movement input is locked for the brief swing window (committed feel)
//     but momentum and gravity continue -- no mid-air stops.
//
// Setup: assign a single sword swing prefab (with Damager + Trigger Collider2D).
// It is instantiated with the correct rotation and offset per direction.
// Right = 0°, Up = 90°, Left = 180°, Down = 270°.
[RequireComponent(typeof(PlayerActions))]
public class PlayerMelee : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject swingPrefab;

    [Header("Offsets — how far from center the swing spawns")]
    [SerializeField] private Vector2 sideOffset = new Vector2(0.6f, 0f);
    [SerializeField] private Vector2 upOffset   = new Vector2(0f,  0.6f);
    [SerializeField] private Vector2 downOffset = new Vector2(0f, -0.6f);

    [Header("Timing")]
    [SerializeField] private float swingDuration  = 0.15f;  // how long the hitbox lives and movement input is blocked
    [SerializeField] private float attackCooldown = 0.35f;  // full time between swings (includes swingDuration)

    private PlayerActions actions;
    private bool facingRight = true;

    // -------------------------------------------------------
    // Awake()
    // Caches PlayerActions.
    // -------------------------------------------------------
    private void Awake()
    {
        actions = GetComponent<PlayerActions>();
    }

    // -------------------------------------------------------
    // Update()
    // Tracks facing direction from horizontal input so the side swing
    // always fires toward where the player last moved.
    // Fires an attack when K is pressed and Attack is available.
    // -------------------------------------------------------
    private void Update()
    {
        float hInput = Input.GetAxisRaw("Horizontal");
        if (hInput > 0f) facingRight = true;
        if (hInput < 0f) facingRight = false;

        if (!actions.CanPerform(PlayerActions.ActionType.Attack)) return;
        if (!Input.GetKeyDown(KeyCode.K)) return;

        StartCoroutine(DoAttack());
    }

    // -------------------------------------------------------
    // DoAttack()
    // Locks Attack for the full cooldown and Movement input for the
    // shorter swing window, then spawns and destroys the hitbox prefab.
    // Momentum is preserved -- only new input is gated.
    // Right = 0°, Up = 90°, Left = 180°, Down = 270°.
    // -------------------------------------------------------
    private IEnumerator DoAttack()
    {
        actions.Lock(PlayerActions.ActionType.Attack);
        actions.Lock(PlayerActions.ActionType.Movement);

        float vInput = Input.GetAxisRaw("Vertical");
        Vector2 offset;
        float rotationZ;

        if (vInput > 0.5f)
        {
            offset    = upOffset;
            rotationZ = 90f;
        }
        else if (vInput < -0.5f)
        {
            offset    = downOffset;
            rotationZ = 270f;
        }
        else if (facingRight)
        {
            offset    = sideOffset;
            rotationZ = 0f;
        }
        else
        {
            offset    = new Vector2(-sideOffset.x, sideOffset.y);
            rotationZ = 180f;
        }

        if (swingPrefab != null)
        {
            Vector3 spawnPos = transform.position + (Vector3)offset;
            Quaternion rotation = Quaternion.Euler(0f, 0f, rotationZ);
            GameObject swing = Instantiate(swingPrefab, spawnPos, rotation);
            swing.GetComponent<TransformTracker>()?.Track(transform);
            Destroy(swing, swingDuration);
        }

        // Release movement input lock after the swing window
        yield return new WaitForSeconds(swingDuration);
        actions.Unlock(PlayerActions.ActionType.Movement);

        // Hold the attack lock for the remaining cooldown
        yield return new WaitForSeconds(attackCooldown - swingDuration);
        actions.Unlock(PlayerActions.ActionType.Attack);
    }
}
