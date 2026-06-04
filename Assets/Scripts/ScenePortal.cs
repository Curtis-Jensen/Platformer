using UnityEngine;
using UnityEngine.SceneManagement;

// -------------------------------------------------------
// ScenePortal
// Teleports the player to a different scene on contact.
// Attach to a GameObject with a Collider2D set to Trigger.
// The target scene must be added to Build Settings.
// -------------------------------------------------------
[RequireComponent(typeof(Collider2D))]
public class ScenePortal : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string playerTag = "Player";

    // -------------------------------------------------------
    // OnTriggerEnter2D(other)
    // Fires when something enters the trigger collider.
    // Checks for the player tag, then loads the target scene.
    // -------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning($"ScenePortal on '{gameObject.name}' has no target scene set.", this);
            return;
        }

        SceneManager.LoadScene(targetScene);
    }
}
