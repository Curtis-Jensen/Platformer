using UnityEngine;

public abstract class Bouncy : MonoBehaviour
{
    [SerializeField] protected float bounceForce = 18f;

    protected void ApplyBounce(Rigidbody2D rb)
    {
        if (rb != null)
            rb.velocity = new Vector2(rb.velocity.x, bounceForce);
    }
}
