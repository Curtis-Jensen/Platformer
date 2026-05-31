using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpCutGravityScale = 4f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerAppearance appearance;
    private bool isGrounded;
    private bool jumpQueued;
    private float coyoteTimer;

    // Counter-based lock: multiple systems can independently lock movement
    // without knowing about each other. Movement is blocked as long as this is > 0.
    private int movementLockCount;

    public bool IsMovementLocked => movementLockCount > 0;

    // -------------------------------------------------------
    // Start()
    // Caches components needed for movement and appearance.
    // -------------------------------------------------------
    private void Start()
    {
        rb             = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        appearance     = GetComponent<PlayerAppearance>();
    }

    // -------------------------------------------------------
    // Update()
    // Reads jump input and ticks the coyote timer down.
    // Clears isGrounded only once the coyote window expires,
    // smoothing over micro-bumps and tile seams.
    // Jump is suppressed while movement is locked.
    // -------------------------------------------------------
    private void Update()
    {
        if (coyoteTimer > 0f)
        {
            coyoteTimer -= Time.deltaTime;
            if (coyoteTimer <= 0f)
                SetGrounded(false);
        }

        if (!IsMovementLocked && isGrounded && Input.GetButtonDown("Jump"))
            jumpQueued = true;

        // Keep the airborne sprite in sync with vertical velocity
        // so the jump→fall transition happens naturally at the apex.
        if (!isGrounded)
            UpdateAirborneSprite();
    }

    // -------------------------------------------------------
    // FixedUpdate()
    // Applies horizontal movement and any queued jump impulse.
    // Both are suppressed while movement is locked.
    // -------------------------------------------------------
    private void FixedUpdate()
    {
        if (IsMovementLocked)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(input * moveSpeed, rb.velocity.y);

        if (jumpQueued)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpQueued = false;
            coyoteTimer = 0f;
            SetGrounded(false); // Pre-emptively unground so the coyote window can't reopen on CollisionExit
        }

        // Variable jump height: releasing the button while still rising cranks up gravity
        // so a tap produces a short hop and a full hold produces the full arc.
        rb.gravityScale = (rb.velocity.y > 0f && !Input.GetButton("Jump")) ? jumpCutGravityScale : 1f;
    }

    // -------------------------------------------------------
    // LockMovement() / UnlockMovement()
    // Increment or decrement the lock counter. Any system that
    // locks movement is responsible for unlocking it when done.
    // Movement stays locked until all locks are released.
    // -------------------------------------------------------
    public void LockMovement()
    {
        movementLockCount++;
    }

    public void UnlockMovement()
    {
        movementLockCount = Mathf.Max(0, movementLockCount - 1);
    }

    // -------------------------------------------------------
    // SetGrounded(bool)
    // Updates grounded state and delegates the visual change
    // to PlayerAppearance.
    // -------------------------------------------------------
    private void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
        if (grounded)
            appearance?.SetSprite(appearance.IdleSprite);
        // When becoming airborne, UpdateAirborneSprite in Update takes over.
    }

    // -------------------------------------------------------
    // UpdateAirborneSprite()
    // Picks jumping or falling sprite each frame based on
    // vertical velocity, so the transition happens naturally
    // at the arc's apex.
    // -------------------------------------------------------
    private void UpdateAirborneSprite()
    {
        appearance?.SetSprite(rb.velocity.y >= 0f ? appearance.JumpingSprite : appearance.FallingSprite);
    }

    // -------------------------------------------------------
    // OnCollisionEnter2D / OnCollisionExit2D
    // Only reacts to objects tagged "Ground".
    // On enter: immediately grounded, cancel any coyote countdown.
    // On exit: start coyote timer instead of clearing grounded right away.
    // -------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Ground")) return;
        coyoteTimer = 0f;
        SetGrounded(true);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Ground")) return;
        coyoteTimer = coyoteTime;
    }
}
