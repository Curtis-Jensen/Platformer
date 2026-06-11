using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnOnDeath;

    // Wire this to Health.onDied in the Inspector
    public void OnDied()
    {
        foreach (var prefab in spawnOnDeath)
            if (prefab != null)
                Instantiate(prefab, transform.position, transform.rotation);

        gameObject.SetActive(false);
    }
}
