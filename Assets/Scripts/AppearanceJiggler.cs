using UnityEngine;

public class AppearanceJiggler : MonoBehaviour
{
    [Tooltip("Max degrees of z-rotation applied in either direction")]
    public float tilt = 5f;

    [Tooltip("Base scale the stretch offsets are applied on top of")]
    public Vector2 baseScale = Vector2.one;

    [Tooltip("Max scale offset applied to x and y independently (e.g. 0.1 = ±10%)")]
    public float stretch = 0.1f;

    SpriteRenderer spriteRenderer;

    // -------------------------------------------------------
    // Awake()
    // Caches the SpriteRenderer so Jiggle() doesn't search at runtime.
    // -------------------------------------------------------
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // -------------------------------------------------------
    // Jiggle()
    // Applies a random gentle appearance change:
    //   - rotates z between -tilt and +tilt degrees
    //   - scales x and y independently within ±stretch
    //   - randomly flips or unflips the sprite on x
    // Called from the editor button or anywhere else that wants a random look.
    // -------------------------------------------------------
    public void Jiggle()
    {
        float rotZ = Random.Range(-tilt, tilt);
        transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);

        float scaleX = Mathf.Max(0f, baseScale.x + Random.Range(-stretch, stretch));
        float scaleY = Mathf.Max(0f, baseScale.y + Random.Range(-stretch, stretch));
        transform.localScale = new Vector3(scaleX, scaleY, 1f);

        if (spriteRenderer != null)
            spriteRenderer.flipX = Random.value > 0.5f;
    }
}
