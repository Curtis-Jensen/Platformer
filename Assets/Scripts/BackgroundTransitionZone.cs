using UnityEngine;

// Place anywhere in the scene as a trigger collider.
// Assign a target background sprite in the Inspector.
// When the player enters, tells BackgroundSwitcher to swap.
[RequireComponent(typeof(Collider2D))]
public class BackgroundTransitionZone : MonoBehaviour
{
    [SerializeField] Sprite targetBackground;

    // -------------------------------------------------------
    // Awake()
    // Ensures this collider is a trigger. Saves the designer
    // from having to remember to check the box.
    // -------------------------------------------------------
    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (targetBackground == null)
            Debug.LogWarning($"BackgroundTransitionZone on {gameObject.name} has no target sprite assigned.");
    }

    // -------------------------------------------------------
    // OnTriggerEnter2D(other)
    // Fires when any collider enters. Player-tagged objects
    // trigger the background swap.
    // -------------------------------------------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (BackgroundSwitcher.Instance == null)
        {
            Debug.LogError("BackgroundTransitionZone: No BackgroundSwitcher found in scene.");
            return;
        }

        BackgroundSwitcher.Instance.SwitchTo(targetBackground);
    }
}
