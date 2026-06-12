using System.Collections;
using UnityEngine;

// Stompable requires a Health component to function.
// Stomping deals 1 damage; the squish plays only when Health reaches zero.
// This means multi-HP enemies take the damage flash on early stomps and squish on the last.
[RequireComponent(typeof(Health))]
public class Stompable : Bouncy
{
    [SerializeField] float squishDuration = 0.15f;

    Collider2D col;
    Health health;

    // -------------------------------------------------------
    // Awake()
    // Caches collider and health, then subscribes to onDied
    // so the squish plays exactly when health hits zero.
    // -------------------------------------------------------
    void Awake()
    {
        col = GetComponent<Collider2D>();
        health = GetComponent<Health>();
        health.onDied.AddListener(OnHealthDepleted);
    }

    // -------------------------------------------------------
    // OnCollisionEnter2D(collision)
    // Detects a stomp: player feet at or above enemy center.
    // Deals 1 damage to Health — the squish happens via onDied,
    // not directly here, so multi-HP enemies survive early stomps.
    // -------------------------------------------------------
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        float playerFeet = collision.collider.bounds.min.y;
        float enemyCenter = col.bounds.center.y;
        if (playerFeet >= enemyCenter)
        {
            ApplyBounce(collision.rigidbody);
            health.TakeDamage(1);
        }
    }

    // -------------------------------------------------------
    // OnHealthDepleted()
    // Called by Health.onDied when HP hits zero.
    // Disables the collider and kicks off the squish animation.
    // -------------------------------------------------------
    void OnHealthDepleted()
    {
        // Capture bounds before disabling — disabled colliders return zeroed bounds
        float bottomY = col.bounds.min.y;
        float halfHeight = col.bounds.extents.y;
        col.enabled = false;
        StartCoroutine(SquishThenDie(bottomY, halfHeight));
    }

    // -------------------------------------------------------
    // SquishThenDie()
    // Lerps localScale Y from current to 0 over squishDuration, then destroys.
    // Pins the bottom of the sprite to the ground by adjusting position each frame.
    // -------------------------------------------------------
    IEnumerator SquishThenDie(float fixedBottomY, float originalHalfHeight)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(startScale.x, 0f, startScale.z);
        float elapsed = 0f;

        while (elapsed < squishDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / squishDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            // Keep the bottom edge fixed: scale the original half-height by how squished we are
            float halfHeight = originalHalfHeight * (1f - t);
            transform.position = new Vector3(transform.position.x, fixedBottomY + halfHeight, transform.position.z);

            yield return null;
        }

        Destroy(gameObject);
    }
}
