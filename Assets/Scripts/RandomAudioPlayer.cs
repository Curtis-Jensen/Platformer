using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    [SerializeField]
    private float minInterval = 2f;
    
    [SerializeField]
    private float maxInterval = 5f;
    
    [SerializeField]
    private float pitchVariation = 0.2f;
    
    private AudioSource audioSource;
    private float timeSinceLastPlay;
    private float nextPlayTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        if (audioSource != null)
        {
            // Apply random pitch variation
            float randomPitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            audioSource.pitch = randomPitch;
            audioSource.Play();
        }
    }

    private void SetNextPlayTime()
    {
        nextPlayTime = Random.Range(minInterval, maxInterval);
    }
}
