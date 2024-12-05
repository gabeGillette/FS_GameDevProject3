using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingObject : MonoBehaviour
{
    public float floatHeight = 1f; // How high the object floats
    public float floatSpeed = 2f;  // Speed of floating up and down
    public float spinSpeed = 100f; // Speed of spinning around the Y-axis

    private Vector3 originalPosition;

    void Start()
    {
        // Store the original position of the object
        originalPosition = transform.position;
    }

    void Update()
    {
        // Floating effect - move the object up and down like a sine wave
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatHeight + originalPosition.y;
        transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);

        // Spinning effect - spin the object around the Y-axis
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }
}
