using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 1;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip healClip;
    [SerializeField] private float hurtPitchVariation = 0.15f;
    [SerializeField] private float healPitchVariation = 0.15f;

    [Header("Damage Flash")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 2;

    public int Current { get; private set; }
    public int Max => maxHealth;

    public UnityEvent onDamaged;
    public UnityEvent onDied;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        Current = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        if (Current <= 0) return;

        Current = Mathf.Max(0, Current - amount);
        onDamaged.Invoke();

        audioSource.PlayWithPitch(hurtClip, hurtPitchVariation);

        if (spriteRenderer != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(DoFlash());
        }

        if (Current == 0)
            onDied.Invoke();
    }

    public void Heal(int amount)
    {
        Current = Mathf.Min(maxHealth, Current + amount);
        audioSource.PlayWithPitch(healClip, healPitchVariation);
    }

    private IEnumerator DoFlash()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
        }

        flashCoroutine = null;
    }
}
