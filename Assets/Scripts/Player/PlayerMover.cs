using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Jump Sprite")]
    [SerializeField] private Sprite jumpingSprite;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Sprite idleSprite;
    private bool isGrounded;
    private bool jumpQueued;
    private float coyoteTimer;

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

        if (isGrounded && Input.GetButtonDown("Jump"))
            jumpQueued = true;
    }

    // -------------------------------------------------------
    // FixedUpdate()
    // Applies horizontal movement and any queued jump impulse.
    // -------------------------------------------------------
    private void FixedUpdate()
    {
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
        if (spriteRenderer == null) return;
        spriteRenderer.sprite = grounded ? idleSprite : (jumpingSprite != null ? jumpingSprite : idleSprite);
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
