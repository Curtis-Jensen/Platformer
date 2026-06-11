using UnityEngine;

// Place on any chair or bench GameObject.
// Player presses E to sit; presses E again to stand up.
public class Chair : Interactable
{
    // Optional: sprite to show on the player while seated.
    // Leave empty and the player just freezes in place with their current sprite.
    [SerializeField] private Sprite sittingSprite;

    private bool isSeatOccupied;

    // Cached so we can restore it when the player stands up.
    private Sprite playerIdleSprite;
    private SpriteRenderer playerSprite;

    // -------------------------------------------------------
    // Interact(PlayerMover)
    // Toggles sit/stand. Snaps player to the chair's x position,
    // swaps their sprite, and locks movement via PlayerMover.
    // -------------------------------------------------------
    public override void Interact(PlayerMover player)
    {
        if (isSeatOccupied)
        {
            StandPlayerUp(player);
        }
        else
        {
            SeatPlayer(player);
        }
    }

    // -------------------------------------------------------
    // SeatPlayer(PlayerMover)
    // Snaps the player to this chair, locks their movement,
    // and swaps their sprite if a sitting sprite is assigned.
    // -------------------------------------------------------
    private void SeatPlayer(PlayerMover player)
    {
        isSeatOccupied = true;
        player.GetComponent<PlayerActions>().Lock(PlayerActions.ActionType.Movement);

        // Snap player's x to the chair's x, keep their current y.
        Vector3 pos = player.transform.position;
        player.transform.position = new Vector3(transform.position.x, pos.y, pos.z);

        if (sittingSprite != null)
        {
            playerSprite = player.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                playerIdleSprite = playerSprite.sprite;
                playerSprite.sprite = sittingSprite;
            }
        }
    }

    // -------------------------------------------------------
    // StandPlayerUp(PlayerMover)
    // Unlocks movement and restores the player's idle sprite.
    // -------------------------------------------------------
    private void StandPlayerUp(PlayerMover player)
    {
        isSeatOccupied = false;
        player.GetComponent<PlayerActions>().Unlock(PlayerActions.ActionType.Movement);

        if (playerSprite != null && playerIdleSprite != null)
        {
            playerSprite.sprite = playerIdleSprite;
            playerSprite = null;
            playerIdleSprite = null;
        }
    }
}
