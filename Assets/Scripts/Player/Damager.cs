using UnityEngine;

// Attach to any object that should deal damage on trigger contact --
// sword swings, projectiles, hazards, etc.
// Physically ignores the player collider on Start so the trigger never
// fires for the player at all -- no tag check needed in OnTriggerEnter2D.
// Lifetime is not managed here -- the spawning system is responsible for
// destroying this object at the right time.
public class Damager : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    // -------------------------------------------------------
    // Start()
    // Finds the player by tag and tells Physics2D to ignore all
    // collisions between this object and the player's collider.
    // -------------------------------------------------------
    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Collider2D playerCol = player.GetComponent<Collider2D>();
        Collider2D myCol = GetComponent<Collider2D>();
        if (playerCol != null && myCol != null)
            Physics2D.IgnoreCollision(myCol, playerCol);
    }

    // -------------------------------------------------------
    // OnTriggerEnter2D(other)
    // Deals damage to anything with a Health component.
    // Player is already excluded via Physics2D.IgnoreCollision in Start.
    // -------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        health?.TakeDamage(damage);
    }
}
