using UnityEngine;

// Follows a target transform each frame, preserving the world-space offset
// from when Track() was called. Useful for effects or hitboxes that should
// stick to their spawner without being parented (which inherits scale/rotation).
public class TransformTracker : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;

    // -------------------------------------------------------
    // Track(target)
    // Begins following the given transform. Captures the current
    // offset so the object stays in the same relative position.
    // -------------------------------------------------------
    public void Track(Transform target)
    {
        this.target = target;
        offset = transform.position - target.position;
    }

    // -------------------------------------------------------
    // Update()
    // Moves this object to match the target's position plus the
    // original offset, every frame.
    // -------------------------------------------------------
    private void Update()
    {
        if (target == null) return;
        transform.position = target.position + offset;
    }
}
