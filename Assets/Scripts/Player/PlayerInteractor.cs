using UnityEngine;

// Detects nearby Interactables and triggers the closest one when E is pressed.
// Attach to the player GameObject alongside PlayerMover.
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRadius = 1.5f;

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
            {
                currentTarget.ShowPrompt(true);
                Debug.Log($"[PlayerInteractor] Target acquired: {currentTarget.gameObject.name}");
            }
            else
            {
                Debug.Log("[PlayerInteractor] No interactable in range.");
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentTarget != null)
            {
                Debug.Log($"[PlayerInteractor] E pressed -- interacting with {currentTarget.gameObject.name}");
                currentTarget.Interact(playerMover);
            }
            else
            {
                Debug.Log("[PlayerInteractor] E pressed but no target in range.");
            }
        }
    }

    // -------------------------------------------------------
    // FindNearest()
    // OverlapCircle to collect all Interactables nearby,
    // returns the one with the smallest distance.
    // Logs raw hit count so we can catch layer mask mismatches.
    // -------------------------------------------------------
    private Interactable FindNearest()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);

        Interactable best = null;
        float bestDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable == null)
            {
                Debug.Log($"[PlayerInteractor] Hit '{hit.gameObject.name}' on layer '{LayerMask.LayerToName(hit.gameObject.layer)}' but no Interactable component found.");
                continue;
            }

            float dist = Vector2.Distance(transform.position, interactable.transform.position);
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
