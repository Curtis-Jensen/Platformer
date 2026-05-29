using UnityEngine;

[System.Serializable]
public class PlayerOutfit
{
    public Sprite idleSprite;
    public Sprite jumpingSprite;
    public Sprite fallingSprite;
}

public class PlayerAppearance : MonoBehaviour
{
    [SerializeField] private PlayerOutfit currentOutfit;

    private SpriteRenderer spriteRenderer;

    public Sprite IdleSprite    => currentOutfit?.idleSprite;
    public Sprite JumpingSprite => currentOutfit?.jumpingSprite;
    public Sprite FallingSprite => currentOutfit?.fallingSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSprite(IdleSprite);
    }

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
