using UnityEngine;

// Place on a bench GameObject. Player presses E to sit; presses E again to stand.
public class BenchSitter : Interactable
{
    // -------------------------------------------------------
    // Interact(PlayerMover)
    // Toggles sitting state on the player. If the player is
    // already sitting (presumably on this bench), stand them up.
    // -------------------------------------------------------
    public override void Interact(PlayerMover player)
    {
        if (player.IsSitting)
            player.StandUp();
        else
            player.Sit(transform.position);
    }
}
