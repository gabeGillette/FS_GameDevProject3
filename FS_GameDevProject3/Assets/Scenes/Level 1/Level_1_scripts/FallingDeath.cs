using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDeath : MonoBehaviour
{
    // Y position threshold that defines when the player is considered to have fallen off the map
    public float fallThreshold = -10f; // The Y position at which the player dies

    // Reference to the player controller (if you have one)
    private playerController player;

    // Reference to the player's Rigidbody (if they have one)
    private Rigidbody rb;

    void Start()
    {
        // Get the playerController component attached to the player object
        player = GetComponent<playerController>();

        // Get the player's Rigidbody to ensure gravity and physics are applied correctly
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody found! Falling detection might not work correctly without Rigidbody.");
        }
    }

   

    void Update()
    {
        // Check if the player's Y position is below the fall threshold
        if (transform.position.y < fallThreshold)
        {
            // Trigger player death
            player.Death();
        }
    }
}
