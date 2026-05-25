using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject corpsePrefab;

    // Wire this to Health.onDied in the Inspector
    public void OnDied()
    {
        if (corpsePrefab != null)
            Instantiate(corpsePrefab, transform.position, transform.rotation);

        gameObject.SetActive(false);
    }
}
