using UnityEngine;

// -------------------------------------------------------
// PlayerAppearance
//
// Owns all visual state for Dawson. Swap out the three
// sprite slots in the Inspector whenever new art lands —
// no code changes needed. PlayerMover (and any other
// scripts that affect air-state) talk to this component
// instead of touching SpriteRenderer directly.
//
// States
//   Idle    – grounded, not moving vertically
//   Jumping – airborne, moving upward  (vy >= 0)
//   Falling – airborne, moving downward (vy < 0)
// -------------------------------------------------------
public class PlayerAppearance : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite jumpingSprite;   // rising
    [SerializeField] private Sprite fallingSprite;   // descending

    private SpriteRenderer spriteRenderer;

    // -------------------------------------------------------
    // Awake – cache renderer and apply idle sprite right away
    // so the Inspector-assigned sprite shows from frame 0.
    // -------------------------------------------------------
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ApplySprite(idleSprite);
    }

    // -------------------------------------------------------
    // Public API – called by PlayerMover (and any other script
    // that knows about Dawson's motion state).
    // -------------------------------------------------------

    /// <summary>Player has landed or is standing on the ground.</summary>
    public void SetIdle()
    {
        ApplySprite(idleSprite);
    }

    /// <summary>Player is airborne and moving upward.</summary>
    public void SetJumping()
    {
        ApplySprite(jumpingSprite);
    }

    /// <summary>Player is airborne and moving downward.</summary>
    public void SetFalling()
    {
        ApplySprite(fallingSprite);
    }

    /// <summary>
    /// Convenience: picks jumping or falling based on the sign of
    /// the vertical velocity. Call this every frame while airborne
    /// so the sprite transitions naturally at the arc's peak.
    /// </summary>
    public void SetAirborne(float verticalVelocity)
    {
        if (verticalVelocity >= 0f)
            SetJumping();
        else
            SetFalling();
    }

    // -------------------------------------------------------
    // Internal helper – falls back to the current sprite when
    // a slot hasn't been assigned yet, so nothing goes blank
    // during development while art is still being wired up.
    // -------------------------------------------------------
    private void ApplySprite(Sprite sprite)
    {
        if (spriteRenderer == null) return;
        if (sprite != null)
            spriteRenderer.sprite = sprite;
    }
}
