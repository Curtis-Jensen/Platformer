using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Sprites")]
    [SerializeField] private Sprite jumpingSprite;
    [SerializeField] private Sprite fallingSprite;
    [SerializeField] private Sprite sittingSprite;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Sprite idleSprite;
    private bool isGrounded;
    private bool jumpQueued;
    private float coyoteTimer;

    public bool IsSitting { get; private set; }

    // -------------------------------------------------------
    // Start()
    // Caches components needed for movement and sprite swapping.
    // -------------------------------------------------------
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        idleSprite = spriteRenderer.sprite;
    }

    // -------------------------------------------------------
    // Update()
    // Reads jump input and ticks the coyote timer down.
    // Clears isGrounded only once the coyote window expires,
    // smoothing over micro-bumps and tile seams.
    // -------------------------------------------------------
    private void Update()
    {
        if (coyoteTimer > 0f)
        {
            coyoteTimer -= Time.deltaTime;
            if (coyoteTimer <= 0f)
                SetGrounded(false);
        }

        UpdateAirborneSprite();

        if (!IsSitting && isGrounded && Input.GetButtonDown("Jump"))
            jumpQueued = true;
    }

    // -------------------------------------------------------
    // FixedUpdate()
    // Applies horizontal movement and any queued jump impulse.
    // Both are suppressed while the player is sitting.
    // -------------------------------------------------------
    private void FixedUpdate()
    {
        if (IsSitting)
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
    }

    // -------------------------------------------------------
    // SetGrounded(bool)
    // Central place to update grounded state and swap sprites.
    // -------------------------------------------------------
    private void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
        if (!grounded) return;
        if (spriteRenderer != null)
            spriteRenderer.sprite = idleSprite;
    }

    // -------------------------------------------------------
    // UpdateAirborneSprite()
    // While airborne, picks jump or fall sprite based on velocity.
    // -------------------------------------------------------
    private void UpdateAirborneSprite()
    {
        if (isGrounded || spriteRenderer == null) return;
        bool rising = rb.velocity.y > 0f;
        Sprite target = rising
            ? (jumpingSprite != null ? jumpingSprite : idleSprite)
            : (fallingSprite != null ? fallingSprite : (jumpingSprite != null ? jumpingSprite : idleSprite));
        spriteRenderer.sprite = target;
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

    // -------------------------------------------------------
    // Sit(Vector2)
    // Snaps the player to the bench position, freezes movement,
    // and swaps to the sitting sprite if one is assigned.
    // -------------------------------------------------------
    public void Sit(Vector2 benchPosition)
    {
        IsSitting = true;
        rb.velocity = Vector2.zero;
        transform.position = new Vector3(benchPosition.x, transform.position.y, transform.position.z);
        if (spriteRenderer != null && sittingSprite != null)
            spriteRenderer.sprite = sittingSprite;
    }

    // -------------------------------------------------------
    // StandUp()
    // Restores normal movement and resets to the idle sprite.
    // -------------------------------------------------------
    public void StandUp()
    {
        IsSitting = false;
        if (spriteRenderer != null)
            spriteRenderer.sprite = idleSprite;
    }
}
