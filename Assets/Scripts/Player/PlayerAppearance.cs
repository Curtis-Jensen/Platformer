using UnityEngine;

// -------------------------------------------------------
// PlayerAppearance
//
// Holds Dawson's sprite assets and owns the SpriteRenderer.
// One method: SetSprite() — callers pass in whichever of
// this component's own sprites they want displayed.
//
// Sprites
//   IdleSprite    – grounded
//   JumpingSprite – airborne, rising  (vy >= 0)
//   FallingSprite – airborne, falling (vy <  0)
// -------------------------------------------------------
public class PlayerAppearance : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite jumpingSprite;
    [SerializeField] private Sprite fallingSprite;

    private SpriteRenderer spriteRenderer;

    public Sprite IdleSprite    => idleSprite;
    public Sprite JumpingSprite => jumpingSprite;
    public Sprite FallingSprite => fallingSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSprite(idleSprite);
    }

    // -------------------------------------------------------
    // SetSprite(Sprite)
    // Single entry-point for all sprite changes. Pass in one
    // of the three properties above; no-ops on null so nothing
    // goes blank while art slots are being filled in.
    // -------------------------------------------------------
    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer == null || sprite == null) return;
        spriteRenderer.sprite = sprite;
    }
}
