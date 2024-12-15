using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    private Vector3 platformOffset;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            // Save the offset of the player relative to the platform
            platformOffset = collision.transform.position - transform.position;

            // Lock the player to the platform by setting it as the child
            transform.SetParent(collision.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }

    void Update()
    {
        if (transform.parent != null)
        {
            // Keep the player in the same relative position to the platform
            transform.position = transform.parent.position - platformOffset;
        }
    }
}
