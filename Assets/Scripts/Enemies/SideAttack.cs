using UnityEngine;

public class SideAttack : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float invincibilityWindow = 0.5f;

    private float lastHitTime = float.MinValue;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        if (IsStomping(col)) return;
        if (Time.time - lastHitTime < invincibilityWindow) return;

        Health health = col.gameObject.GetComponent<Health>();
        if (health == null) return;

        health.TakeDamage(damage);
        lastHitTime = Time.time;
    }

    // Player's feet at or above the enemy center = stomp, not a side hit
    private bool IsStomping(Collision2D col)
    {
        float playerFeet = col.collider.bounds.min.y;
        float enemyCenter = GetComponent<Collider2D>().bounds.center.y;
        return playerFeet >= enemyCenter;
    }
}
