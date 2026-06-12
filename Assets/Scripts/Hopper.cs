using UnityEngine;

// Fires a periodic hop impulse in a random direction when grounded.
// Pair with Stompable (and optionally DirectionFlipper) for a full hopping enemy.
public class Hopper : MonoBehaviour
{
    [SerializeField] private float minHopForce = 5f;
    [SerializeField] private float maxHopForce = 9f;
    [SerializeField] private float minHopInterval = 0.6f;
    [SerializeField] private float maxHopInterval = 1.4f;
    [SerializeField] private float fallGravityMultiplier = 0f;

    [Header("Hop Sprite")]
    [SerializeField] private Sprite hoppingSprite;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Sprite idleSprite;
    private int groundContacts;
    private float hopTimer;

    private bool isGrounded => groundContacts > 0;

    // -------------------------------------------------------
    // Awake()
    // Caches components and seeds the first hop timer so
    // multiple hoppers spawned at the same time don't all
    // jump in unison.
    // -------------------------------------------------------
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            idleSprite = spriteRenderer.sprite;

        hopTimer = Random.Range(minHopInterval, maxHopInterval);
    }

    // -------------------------------------------------------
    // Update()
    // Counts down the hop timer. Fires a hop when grounded
    // and the timer expires, then resets to a new random interval.
    // -------------------------------------------------------
    private void FixedUpdate()
    {
        if (fallGravityMultiplier > 0f && rb.velocity.y < 0f)
            rb.AddForce(Physics2D.gravity * fallGravityMultiplier, ForceMode2D.Force);
    }

    private void Update()
    {
        if (!isGrounded) return;

        hopTimer -= Time.deltaTime;
        if (hopTimer <= 0f)
        {
            Hop();
            hopTimer = Random.Range(minHopInterval, maxHopInterval);
        }
    }

    // -------------------------------------------------------
    // Hop()
    // Picks a random horizontal direction and applies an impulse
    // at a random angle between straight up and 45 degrees to
    // either side. Zeroes existing velocity first for consistency.
    // Swaps to the hop sprite; idle sprite is restored on landing.
    // -------------------------------------------------------
    private void Hop()
    {
        float angle = Random.Range(-45f, 45f);
        Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.up;

        float force = Random.Range(minHopForce, maxHopForce);
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        if (spriteRenderer != null && hoppingSprite != null)
            spriteRenderer.sprite = hoppingSprite;
    }

    // -------------------------------------------------------
    // OnCollisionEnter2D / OnCollisionExit2D
    // Only reacts to objects tagged "Ground" to track grounded
    // state and restore the idle sprite on landing.
    // -------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D col)
    {
        groundContacts++;
        if (spriteRenderer != null && idleSprite != null)
            spriteRenderer.sprite = idleSprite;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        groundContacts = Mathf.Max(0, groundContacts - 1);
    }
}
