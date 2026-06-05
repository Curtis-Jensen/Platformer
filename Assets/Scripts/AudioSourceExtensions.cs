using UnityEngine;

public static class AudioSourceExtensions
{
    // -------------------------------------------------------
    // PlayWithPitch(source, variation)
    // Plays the AudioSource with a randomized pitch centered on its current base pitch.
    // Call this instead of audioSource.Play() anywhere you want pitch variation.
    // Returns the actual pitch used so callers can time coroutines to clip duration.
    // -------------------------------------------------------
    public static float PlayWithPitch(this AudioSource source, float variation = 0.15f)
    {
        if (source == null || source.clip == null) return 1f;

        float basePitch = source.pitch;
        float randomPitch = basePitch + Random.Range(-variation, variation);
        source.pitch = randomPitch;
        source.Play();
        return randomPitch;
    }
}
