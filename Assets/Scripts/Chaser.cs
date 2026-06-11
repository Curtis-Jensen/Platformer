using UnityEngine;

// Attach to any NPC or object that should walk toward the player and stop nearby.
public class Chaser : MonoBehaviour
{
    [Header("Tag")]
    [SerializeField] private string targetTag = "Player";

    [Header("Movement")]
    public float stoppingDistance = 2f;
    [SerializeField] private float moveSpeed = 3f;

    private Transform target;

    // -------------------------------------------------------
    // Start()
    // Finds the player by tag and caches the SpriteRenderer
    // so we can flip it each frame without a GetComponent call.
    // -------------------------------------------------------
    private void Start()
    {
        GameObject player = GameObject.FindWithTag(targetTag);
        if (player != null)
            target = player.transform;

    }

    // -------------------------------------------------------
    // Update()
    // Moves this object toward the player each frame.
    // Stops when within stoppingDistance.
    // Flips the sprite so it always faces the player.
    // -------------------------------------------------------
    private void Update()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= stoppingDistance) return;

        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
#endif
}
