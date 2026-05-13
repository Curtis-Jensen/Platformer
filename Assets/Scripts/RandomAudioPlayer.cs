using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    [SerializeField] private float minInterval = 2f;
    [SerializeField] private float maxInterval = 5f;
    [SerializeField] private float pitchVariation = 0.2f;

    [Header("Moo Sprite")]
    [SerializeField] private Sprite mooingSprite;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Sprite idleSprite;
    private float timeSinceLastPlay;
    private float nextPlayTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        idleSprite = spriteRenderer.sprite;
        SetNextPlayTime();
    }

    private void Update()
    {
        timeSinceLastPlay += Time.deltaTime;

        if (timeSinceLastPlay >= nextPlayTime)
        {
            PlaySound();
            SetNextPlayTime();
            timeSinceLastPlay = 0f;
        }
    }

    private void PlaySound()
    {
        if (audioSource == null) return;

        float randomPitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.pitch = randomPitch;
        audioSource.Play();

        if (spriteRenderer != null && mooingSprite != null)
            StartCoroutine(SwapSprite(audioSource.clip.length / randomPitch));
    }

    private IEnumerator SwapSprite(float duration)
    {
        spriteRenderer.sprite = mooingSprite;
        yield return new WaitForSeconds(duration);
        spriteRenderer.sprite = idleSprite;
    }

    private void SetNextPlayTime()
    {
        nextPlayTime = Random.Range(minInterval, maxInterval);
    }
}
