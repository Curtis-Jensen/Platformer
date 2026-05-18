using System.Collections;
using UnityEngine;

public class Stompable : MonoBehaviour
{
    [SerializeField] float squishDuration = 0.15f;

    Collider2D col;

    // -------------------------------------------------------
    // Awake()
    // Caches collider so we can disable it immediately on stomp.
    // -------------------------------------------------------
    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    // -------------------------------------------------------
    // OnCollisionEnter2D(collision)
    // Detects stomp: player feet above enemy center.
    // Disables collider and starts squish animation before destroy.
    // -------------------------------------------------------
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        float playerFeet = collision.collider.bounds.min.y;
        float enemyCenter = col.bounds.center.y;
        if (playerFeet >= enemyCenter)
        {
            col.enabled = false;
            StartCoroutine(SquishThenDie());
        }
    }

    // -------------------------------------------------------
    // SquishThenDie()
    // Lerps localScale Y from current to 0 over squishDuration, then destroys.
    // Pins the bottom of the sprite to the ground by adjusting position each frame.
    // -------------------------------------------------------
    IEnumerator SquishThenDie()
    {
        float fixedBottomY = col.bounds.min.y;
        float originalHalfHeight = col.bounds.extents.y;
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
