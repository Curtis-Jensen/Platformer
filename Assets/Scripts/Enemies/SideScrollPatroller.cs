using UnityEngine;

public class SideScrollPatroller : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;
    private float currentDirection = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float actualX = rb.velocity.x;
        if (Mathf.Abs(actualX) < moveSpeed * 0.5f)
        {
            currentDirection *= -1f;
        }

        rb.velocity = new Vector2(moveSpeed * currentDirection, rb.velocity.y);
    }
}
