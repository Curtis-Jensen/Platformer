using System.Collections.Generic;
using UnityEngine;

// Add to a moving platform alongside PlatformMover.
// Tracks passengers via collision and manually applies the
// platform's movement delta to their Rigidbody2D each frame.
// This avoids Rigidbody2D parenting, which Unity ignores for physics.
[RequireComponent(typeof(Rigidbody2D))]
public class PlatformPassengerCarrier : MonoBehaviour
{
    [SerializeField] private string passengerTag = "Player";

    private Rigidbody2D rb;
    private Vector2 previousPosition;
    private readonly HashSet<Rigidbody2D> passengers = new HashSet<Rigidbody2D>();

    // -------------------------------------------------------
    // Awake()
    // Caches the Rigidbody2D and records the starting position
    // so the first delta is zero.
    // -------------------------------------------------------
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        previousPosition = rb.position;
    }

    // -------------------------------------------------------
    // FixedUpdate()
    // Calculates how far the platform moved this physics step
    // and nudges every passenger by the same offset so they
    // ride along without sliding.
    // -------------------------------------------------------
    private void FixedUpdate()
    {
        Vector2 delta = rb.position - previousPosition;
        previousPosition = rb.position;

        if (delta == Vector2.zero) return;

        foreach (Rigidbody2D passenger in passengers)
        {
            if (passenger == null) continue;
            passenger.transform.position += (Vector3)delta;
        }
    }

    // -------------------------------------------------------
    // OnCollisionEnter2D / OnCollisionExit2D
    // Registers and deregisters passengers by tag.
    // -------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag(passengerTag)) return;
        Rigidbody2D passengerRb = col.gameObject.GetComponent<Rigidbody2D>();
        if (passengerRb != null)
            passengers.Add(passengerRb);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag(passengerTag)) return;
        Rigidbody2D passengerRb = col.gameObject.GetComponent<Rigidbody2D>();
        if (passengerRb != null)
            passengers.Remove(passengerRb);
    }
}
