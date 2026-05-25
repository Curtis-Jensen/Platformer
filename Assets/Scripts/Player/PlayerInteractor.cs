using UnityEngine;

// Detects nearby Interactables and triggers the closest one when E is pressed.
// Attach to the player GameObject alongside PlayerMover.
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask interactableLayers;

    private PlayerMover playerMover;
    private Interactable currentTarget;

    // -------------------------------------------------------
    // Start()
    // Caches PlayerMover so Interact() can pass it along.
    // -------------------------------------------------------
    private void Start()
    {
        playerMover = GetComponent<PlayerMover>();
    }

    // -------------------------------------------------------
    // Update()
    // Each frame: finds the nearest Interactable in range,
    // manages the prompt on it, and checks for E input.
    // -------------------------------------------------------
    private void Update()
    {
        Interactable nearest = FindNearest();

        if (nearest != currentTarget)
        {
            if (currentTarget != null)
                currentTarget.ShowPrompt(false);

            currentTarget = nearest;

            if (currentTarget != null)
                currentTarget.ShowPrompt(true);
        }

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
            currentTarget.Interact(playerMover);
    }

    // -------------------------------------------------------
    // FindNearest()
    // OverlapCircle to collect all Interactables nearby,
    // returns the one with the smallest distance.
    // -------------------------------------------------------
    private Interactable FindNearest()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayers);

        Interactable best = null;
        float bestDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable == null) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = interactable;
            }
        }

        return best;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
