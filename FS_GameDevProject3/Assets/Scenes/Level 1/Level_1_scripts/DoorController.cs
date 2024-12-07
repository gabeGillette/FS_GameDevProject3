using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float forceMultiplier = 10f;  // How much force is applied when the player presses on the door
    private HingeJoint doorHinge;  // The hinge joint attached to the door
    private Rigidbody doorRigidbody;  // The rigidbody of the door

    void Start()
    {
        // Get the HingeJoint and Rigidbody components
        doorHinge = GetComponent<HingeJoint>();
        doorRigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        // Detect if the player is pressing against the door (you can use a tag to identify the player)
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply force in the direction the player is pressing
            Vector3 pushDirection = collision.contacts[0].normal;  // Get the normal direction of the contact point
            doorRigidbody.AddForce(pushDirection * forceMultiplier, ForceMode.Force);
        }
    }
}
