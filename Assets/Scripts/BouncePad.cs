using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] float bounceForce = 20f;
    [SerializeField] float squishDuration = 0.15f;
    [SerializeField] Vector2 squishScale = new Vector2(1.4f, 0.6f);

    Vector3 restScale;
    float squishTimer;

    void Start()
    {
        restScale = transform.localScale;
    }

    void Update()
    {
        if (squishTimer > 0f)
        {
            squishTimer -= Time.deltaTime;
            float t = squishTimer / squishDuration;
            // t goes 1→0: lerp from squished back to rest
            transform.localScale = Vector3.Lerp(restScale, new Vector3(squishScale.x, squishScale.y, 1f) * restScale.x, t);
        }
        else
        {
            transform.localScale = restScale;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Rigidbody2D rb = col.rigidbody;
        if (rb == null) return;

        // Only bounce things landing on top
        float thingBottom = col.collider.bounds.min.y;
        float padTop = GetComponent<Collider2D>().bounds.max.y;
        if (thingBottom < padTop - 0.1f) return;

        rb.velocity = new Vector2(rb.velocity.x, bounceForce);
        squishTimer = squishDuration;
    }
}
