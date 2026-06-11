using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerActions))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpCutGravityScale = 4f;

    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private float jumpPitchVariation = 0.1f;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private PlayerAppearance appearance;
    private PlayerActions actions;
    private bool isGrounded;
    private bool jumpQueued;
    private float coyoteTimer;

    // -------------------------------------------------------
    // Start()
    // Caches components needed for movement and appearance.
    // -------------------------------------------------------
    private void Start()
    {
        rb          = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        appearance  = GetComponent<PlayerAppearance>();
        actions     = GetComponent<PlayerActions>();
    }

    // -------------------------------------------------------
    // Update()
    // Reads jump input and ticks the coyote timer down.
    // Jump input is only accepted when Movement is unlocked.
    // -------------------------------------------------------
    private void Update()
    {
        if (coyoteTimer > 0f)
        {
            coyoteTimer -= Time.deltaTime;
            if (coyoteTimer <= 0f)
                SetGrounded(false);
        }

        if (actions.CanPerform(PlayerActions.ActionType.Movement) && isGrounded && Input.GetButtonDown("Jump"))
            jumpQueued = true;

        if (!isGrounded)
            UpdateAirborneSprite();
    }

    // -------------------------------------------------------
    // FixedUpdate()
    // When Movement is locked, skips applying horizontal input
    // but lets existing momentum and gravity run freely --
    // the player keeps their arc instead of stopping mid-air.
    // When unlocked, drives horizontal velocity from input as normal.
    // -------------------------------------------------------
    private void FixedUpdate()
    {
        if (actions.CanPerform(PlayerActions.ActionType.Movement))
        {
            float input = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(input * moveSpeed, rb.velocity.y);
        }

        if (jumpQueued)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpQueued = false;
            audioSource.PlayWithPitch(jumpClip, jumpPitchVariation);
            coyoteTimer = 0f;
            SetGrounded(false);
        }

        // Variable jump height: releasing Jump while rising cranks gravity so
        // a tap produces a short hop and a full hold gives the full arc.
        rb.gravityScale = (rb.velocity.y > 0f && !Input.GetButton("Jump")) ? jumpCutGravityScale : 1f;
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
    }

    // -------------------------------------------------------
    // UpdateAirborneSprite()
    // Picks jumping or falling sprite each frame based on
    // vertical velocity so the transition happens naturally at the apex.
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
