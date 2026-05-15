using UnityEngine;

/// Follows the player upward only -- never scrolls back down.
/// Attach to the Camera. Assign the player Transform in the Inspector.
public class ClimberCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [Tooltip("How far above center the player sits before the camera starts following")]
    [SerializeField] private float deadZoneAbove = 1f;
    [Tooltip("Smooth follow speed (higher = snappier)")]
    [SerializeField] private float smoothSpeed = 5f;

    private float highestY;

    private void Start()
    {
        highestY = transform.position.y;
    }

    private void LateUpdate()
    {
        float targetY = player.position.y - deadZoneAbove;

        if (targetY > highestY)
            highestY = targetY;

        float newY = Mathf.Lerp(transform.position.y, highestY, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
