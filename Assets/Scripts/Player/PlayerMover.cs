using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.1f;

    private Rigidbody2D rb;
    private PlayerAppearance appearance;
    private bool isGrounded;
    private bool jumpQueued;
    private float coyoteTimer;

    // -------------------------------------------------------
    // Start()
    // Caches components needed for movement and appearance.
    // -------------------------------------------------------
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        appearance = GetComponent<PlayerAppearance>();
    }

    // -------------------------------------------------------
    // Update()
    // Reads jump input and ticks the coyote timer down.
    // While airborne, tells PlayerAppearance to pick the right
    // sprite based on whether Dawson is rising or falling.
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

        // Keep the airborne sprite in sync with vertical velocity
        // so the jump→fall transition happens naturally at the apex.
        if (!isGrounded)
            appearance?.SetAirborne(rb.velocity.y);
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
    // Updates grounded state and delegates the visual change
    // to PlayerAppearance.
    // -------------------------------------------------------
    private void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
        if (grounded)
            appearance?.SetIdle();
        // When becoming airborne the velocity-driven Update loop
        // takes over and picks jumping vs. falling each frame.
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
