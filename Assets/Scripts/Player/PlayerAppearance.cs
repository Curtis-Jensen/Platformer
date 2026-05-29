using UnityEngine;

[System.Serializable]
public class PlayerOutfit
{
    public string name;
    public Sprite idleSprite;
    public Sprite jumpingSprite;
    public Sprite fallingSprite;
}

public class PlayerAppearance : MonoBehaviour
{
    [SerializeField] private PlayerOutfit[] outfits;
    [SerializeField] private int outfitIndex;

    private PlayerOutfit currentOutfit;
    private SpriteRenderer spriteRenderer;

    public Sprite IdleSprite    => currentOutfit?.idleSprite;
    public Sprite JumpingSprite => currentOutfit?.jumpingSprite;
    public Sprite FallingSprite => currentOutfit?.fallingSprite;

    // -------------------------------------------------------
    // Awake()
    // Caches the SpriteRenderer and equips the outfit selected
    // in the Inspector (outfitIndex into the outfits array).
    // -------------------------------------------------------
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        WearOutfit(outfitIndex);
    }

    // -------------------------------------------------------
    // WearOutfit(int index)
    // Equips the outfit at the given index in the outfits array.
    // Clamps silently if index is out of range.
    // -------------------------------------------------------
    public void WearOutfit(int index)
    {
        if (outfits == null || outfits.Length == 0) return;
        outfitIndex = Mathf.Clamp(index, 0, outfits.Length - 1);
        WearOutfit(outfits[outfitIndex]);
    }

    // -------------------------------------------------------
    // WearOutfit(PlayerOutfit outfit)
    // Directly equips any outfit instance (used by WearOutfit(int)
    // and any external caller that already has an outfit reference).
    // -------------------------------------------------------
    public void WearOutfit(PlayerOutfit outfit)
    {
        currentOutfit = outfit;
        SetSprite(IdleSprite);
    }

    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer == null || sprite == null) return;
        spriteRenderer.sprite = sprite;
    }
}
