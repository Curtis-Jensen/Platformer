using UnityEngine;

// Keeps the AudioListener at Z=0 so 3D audio distance is calculated in 2D (X/Y) only.
// Without this, the camera's Z offset (~10 units) means all sounds are already at the
// max distance before any horizontal separation is factored in.
[RequireComponent(typeof(AudioListener))]
public class AudioListenerZLock : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
