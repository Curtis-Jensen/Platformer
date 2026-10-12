using UnityEngine;

// Sits on the camera (or any persistent GameObject).
// Owns the background SpriteRenderer and swaps its sprite on demand.
// BackgroundTransitionZone triggers SwitchTo() when the player enters.
public class BackgroundSwitcher : MonoBehaviour
{
    public static BackgroundSwitcher Instance { get; private set; }

    [SerializeField] SpriteRenderer background;

    // -------------------------------------------------------
    // Awake()
    // Registers the singleton. Fails loudly if background is
    // not assigned in the Inspector.
    // -------------------------------------------------------
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple BackgroundSwitchers in scene -- destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (background == null)
            Debug.LogError("BackgroundSwitcher: background SpriteRenderer not assigned.");
    }

    // -------------------------------------------------------
    // SwitchTo(newSprite)
    // Immediately swaps the background sprite. Called by
    // BackgroundTransitionZone on player entry.
    // -------------------------------------------------------
    public void SwitchTo(Sprite newSprite)
    {
        if (newSprite == null) return;
        background.sprite = newSprite;
    }
}
