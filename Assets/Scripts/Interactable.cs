using UnityEngine;

// Base class for anything the player can press E to interact with.
// Extend this, override Interact(), and drop it on a GameObject.
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] public string promptText = "Press E";

    // Optional world-space prompt object (e.g. a Canvas with "Press E" text).
    // Wire one up in the prefab, or leave empty -- both work.
    [SerializeField] private GameObject promptObject;

    // -------------------------------------------------------
    // ShowPrompt(bool)
    // Called by PlayerInteractor when the player enters or
    // leaves interaction range.
    // -------------------------------------------------------
    public void ShowPrompt(bool visible)
    {
        if (promptObject != null)
            promptObject.SetActive(visible);
    }

    // -------------------------------------------------------
    // Interact(PlayerMover)
    // Override in subclasses to define what happens when
    // the player presses E near this object.
    // -------------------------------------------------------
    public abstract void Interact(PlayerMover player);
}
