using UnityEngine;

// Drop on any GameObject and wire its Play() method to a UnityEvent (e.g. Health.onDamaged).
[RequireComponent(typeof(AudioSource))]
public class AudioEventPlayer : MonoBehaviour
{
    [SerializeField] private float pitchVariation = 0.15f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // -------------------------------------------------------
    // Play()
    // Plays the attached AudioSource with pitch variation.
    // Intended to be wired up in the Inspector via UnityEvents.
    // -------------------------------------------------------
    public void Play()
    {
        audioSource.PlayWithPitch(pitchVariation);
    }
}
