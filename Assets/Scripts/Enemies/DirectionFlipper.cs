using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DirectionFlipper : MonoBehaviour
{
    [Tooltip("The minimum distance traveled to register a direction change")]
    public float movementThreshold = 0.01f;
    public bool initiallyFacingRight;

    private Vector3 previousPosition;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousPosition = transform.position;
        facingRight = initiallyFacingRight;
    }

    void LateUpdate()
    {
        // Calculate movement direction
        Vector3 movementDelta = transform.position - previousPosition;
        
        // Only process if there's significant movement
        if (movementDelta.magnitude > movementThreshold)
        {
            // Determine if moving right or left
            bool movingRight = movementDelta.x > 0;
            
            // Check if direction differs from current facing direction
            if (movingRight && !facingRight)
            {
                Flip();
            }
            else if (!movingRight && facingRight)
            {
                Flip();
            }
            
            previousPosition = transform.position;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
