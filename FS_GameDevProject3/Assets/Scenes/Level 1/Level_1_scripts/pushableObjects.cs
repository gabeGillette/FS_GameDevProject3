using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushableObjects : MonoBehaviour
{
    public float pushStrength = 10f;    // The force with which the object is pushed.
    public float maxPushSpeed = 5f;     // Maximum speed the object can reach when pushed.

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Lock rotation on all axes to prevent tumbling
        rb.freezeRotation = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        // When the player is in contact with the object
        if (collision.collider.CompareTag("Player"))
        {
            // Calculate the direction to push the object (based on the player's forward direction)
            Vector3 pushDirection = collision.transform.forward;

            // Remove any vertical component (Y-axis) to make the object slide horizontally
            pushDirection.y = 0;

            // Apply force in the direction the player is facing.
            rb.AddForce(pushDirection * pushStrength, ForceMode.Force);

            // Limit the velocity of the object to avoid it moving too fast
            if (rb.velocity.magnitude > maxPushSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxPushSpeed;
            }
        }
    }
}
