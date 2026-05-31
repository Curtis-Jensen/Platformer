using UnityEngine;
using UnityEngine.Events;

// -------------------------------------------------------
// HealthRestorer
// A pickup that fully restores health on contact.
// Attach to a GameObject with a Collider2D set to trigger.
// Destroys itself after healing the player.
// -------------------------------------------------------
public class HealthRestorer : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    public UnityEvent onPickedUp;

    // -------------------------------------------------------
    // OnTriggerEnter2D(other)
    // Fires when something enters the trigger collider.
    // Checks for the player tag, heals to full, then self-destructs.
    // -------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        Health health = other.GetComponent<Health>();
        if (health == null) return;

        health.Heal(health.Max);
        onPickedUp.Invoke();
        Destroy(gameObject);
    }
}
