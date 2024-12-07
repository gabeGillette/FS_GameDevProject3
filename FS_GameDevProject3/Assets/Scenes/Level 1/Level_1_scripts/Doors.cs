using UnityEngine;

public class Doors : MonoBehaviour, IInteractable
{
    public float slideDistance = 5f;    // How far the door should slide (if sliding is enabled)
    public float slideSpeed = 2f;       // Speed at which the door slides (if sliding is enabled)
    public float swingAngle = 90f;      // Angle the door should rotate when swinging
    public float swingSpeed = 2f;      // Speed at which the door swings (if swinging is enabled)

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isSliding = false;
    private bool isSwinging = false;
    private bool isOpen = false;
    public bool slideToRight = true;    // If true, the door slides to the right, else to the left
    public bool swingDoor = true;       // If true, the door will swing open instead of sliding

    public void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Set up sliding target position
        targetPosition = slideToRight ? initialPosition + Vector3.forward * slideDistance : initialPosition - Vector3.forward * slideDistance;

        // Set up swinging target rotation
        targetRotation = initialRotation * Quaternion.Euler(0f, swingAngle, 0f);
    }

    public void Update()
    {
        if (isSliding)
        {
            // Move the door smoothly towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);

            // Once the door reaches its target position, stop sliding
            if (transform.position == targetPosition)
            {
                isSliding = false;
            }
        }
        else if (isSwinging)
        {
            // Rotate the door smoothly towards the target rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, swingSpeed * Time.deltaTime);

            // Once the door reaches its target rotation, stop swinging
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                isSwinging = false;
            }
        }
    }

    public void Interact()
    {
        // If the door should slide
        if (!swingDoor)
        {
            if (!isSliding)
            {
                isSliding = true;
                if (isOpen)
                {
                    // Close the door by sliding it back to its original position
                    targetPosition = initialPosition;
                }
                else
                {
                    // Open the door by sliding it to the target position
                    targetPosition = slideToRight ? initialPosition + Vector3.forward * slideDistance : initialPosition - Vector3.forward * slideDistance;
                }
                isOpen = !isOpen;
            }
        }
        // If the door should swing open
        else
        {
            if (!isSwinging)
            {
                isSwinging = true;
                if (isOpen)
                {
                    // Close the door by rotating it back to its original position
                    targetRotation = initialRotation;
                }
                else
                {
                    // Open the door by rotating it to the target rotation
                    targetRotation = initialRotation * Quaternion.Euler(0f, swingAngle, 0f);
                }
                isOpen = !isOpen;
            }
        }
    }
}