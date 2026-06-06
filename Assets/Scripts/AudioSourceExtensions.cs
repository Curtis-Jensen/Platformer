using UnityEngine;

public static class AudioSourceExtensions
{
    // -------------------------------------------------------
    // PlayWithPitch(source, variation)
    // Plays the AudioSource with a randomized pitch centered on its current base pitch.
    // Call this instead of audioSource.Play() anywhere you want pitch variation.
    // Returns the actual pitch used so callers can time coroutines to clip duration.
    // -------------------------------------------------------
    // -------------------------------------------------------
    // PlayWithPitch(source, variation, basePitch)
    // Plays source.clip with a randomized pitch. Use when a script owns
    // its AudioSource exclusively (e.g. RandomAudioPlayer, BouncePad).
    // Always randomizes from basePitch — never reads source.pitch — to prevent drift.
    // -------------------------------------------------------
    public static float PlayWithPitch(this AudioSource source, float variation = 0.15f, float basePitch = 1f)
    {
        if (source == null || source.clip == null) return 1f;

        float randomPitch = basePitch + Random.Range(-variation, variation);
        source.pitch = randomPitch;
        source.Play();
        return randomPitch;
    }

    // -------------------------------------------------------
    // PlayWithPitch(source, clip, variation, basePitch)
    // Plays a specific clip via PlayOneShot so multiple sounds can overlap
    // on a shared AudioSource without interrupting each other.
    // Use when several scripts share one AudioSource on the same GameObject.
    // -------------------------------------------------------
    public static void PlayWithPitch(this AudioSource source, AudioClip clip, float variation = 0.15f, float basePitch = 1f)
    {
        if (source == null || clip == null) return;

        source.pitch = basePitch + Random.Range(-variation, variation);
        source.PlayOneShot(clip);
    }
}
