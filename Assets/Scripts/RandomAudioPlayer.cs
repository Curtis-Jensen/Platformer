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
    private float basePitch;
    private float timeSinceLastPlay;
    private float nextPlayTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        idleSprite = spriteRenderer.sprite;
        basePitch = audioSource.pitch;
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

        float randomPitch = basePitch + Random.Range(-pitchVariation, pitchVariation);
        audioSource.pitch = randomPitch;
        audioSource.Play();

        float clipDuration = audioSource.clip.length / randomPitch;
        if (spriteRenderer != null && mooingSprite != null)
            StartCoroutine(SwapSprite(clipDuration));
        else
            StartCoroutine(ResetPitchAfter(clipDuration));
    }

    private IEnumerator SwapSprite(float duration)
    {
        spriteRenderer.sprite = mooingSprite;
        yield return new WaitForSeconds(duration);
        spriteRenderer.sprite = idleSprite;
        audioSource.pitch = basePitch;
    }

    private IEnumerator ResetPitchAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        audioSource.pitch = basePitch;
    }

    private void SetNextPlayTime()
    {
        nextPlayTime = Random.Range(minInterval, maxInterval);
    }
}
