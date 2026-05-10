using UnityEngine;

/// <summary>
/// Flips a sprite based on the horizontal velocity of a Rigidbody2D.
/// Generic and reusable for any 2D entity that moves side-to-side.
/// </summary>
public class DirectionFlipper : MonoBehaviour
{
    [SerializeField] private float velocityThreshold = 0.1f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            Debug.LogError("DirectionFlipper requires a Rigidbody2D component!");
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("DirectionFlipper requires a SpriteRenderer component!");
        }
    }

    private void Update()
    {
        // Check horizontal velocity and flip sprite accordingly
        if (rb.velocity.x > velocityThreshold && !facingRight)
        {
            FaceRight();
        }
        else if (rb.velocity.x < -velocityThreshold && facingRight)
        {
            FaceLeft();
        }
    }

    private void FaceRight()
    {
        facingRight = true;
        spriteRenderer.flipX = false;
    }

    private void FaceLeft()
    {
        facingRight = false;
        spriteRenderer.flipX = true;
    }
}
