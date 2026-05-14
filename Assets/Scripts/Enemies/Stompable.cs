using UnityEngine;

public class Stompable : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        // Player's feet must be above the enemy's center to count as a stomp
        float playerFeet = col.collider.bounds.min.y;
        float enemyCenter = GetComponent<Collider2D>().bounds.center.y;
        if (playerFeet >= enemyCenter)
            Destroy(gameObject);
    }
}
