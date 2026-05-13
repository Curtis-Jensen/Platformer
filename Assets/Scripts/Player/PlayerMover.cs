using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Jump Sprite")]
    [SerializeField] private Sprite jumpingSprite;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Sprite idleSprite;
    private bool isGrounded;
    private bool jumpQueued;

    // -------------------------------------------------------
    // Start()
    // Caches the Rigidbody2D component.
    // -------------------------------------------------------
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        idleSprite = spriteRenderer.sprite;
    }

    // -------------------------------------------------------
    // Update()
    // Reads jump input. Queues the jump so FixedUpdate can
    // apply it on the next physics step.
    // -------------------------------------------------------
    private void Update()
    {
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
        }
    }

    // -------------------------------------------------------
    // OnCollisionEnter2D / OnCollisionExit2D
    // Dead-simple ground detection: touching anything = grounded.
    // TECH DEBT: No layer filtering — will count walls and enemies
    // as ground. Propose: add a groundLayer mask when it becomes
    // a problem.
    // -------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D col)
    {
        isGrounded = true;
        if (spriteRenderer != null) spriteRenderer.sprite = idleSprite;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        isGrounded = false;
        if (spriteRenderer != null && jumpingSprite != null) spriteRenderer.sprite = jumpingSprite;
    }
}
