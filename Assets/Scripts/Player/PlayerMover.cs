using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 10f;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _jumpQueued;

    // -------------------------------------------------------
    // Start()
    // Caches the Rigidbody2D component.
    // -------------------------------------------------------
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // -------------------------------------------------------
    // Update()
    // Reads jump input. Queues the jump so FixedUpdate can
    // apply it on the next physics step.
    // -------------------------------------------------------
    private void Update()
    {
        if (_isGrounded && Input.GetButtonDown("Jump"))
            _jumpQueued = true;
    }

    // -------------------------------------------------------
    // FixedUpdate()
    // Applies horizontal movement and any queued jump impulse.
    // -------------------------------------------------------
    private void FixedUpdate()
    {
        float input = Input.GetAxisRaw("Horizontal");
        _rb.velocity = new Vector2(input * _moveSpeed, _rb.velocity.y);

        if (_jumpQueued)
        {
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _jumpQueued = false;
        }
    }

    // -------------------------------------------------------
    // OnCollisionEnter2D / OnCollisionExit2D
    // Dead-simple ground detection: touching anything = grounded.
    // TECH DEBT: No layer filtering — will count walls and enemies
    // as ground. Propose: add a groundLayer mask when it becomes
    // a problem.
    // -------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D col) => _isGrounded = true;
    private void OnCollisionExit2D(Collision2D col) => _isGrounded = false;
}
