using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour, IInteractable
{

    public float slideDistance = 5f;  // How far the door should slide
    public float slideSpeed = 2f;     // Speed at which the door slides

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isSliding = false;
    private bool isOpen = false;
    public bool slideToRight = true;  // If true, the door slides to the right, else to the left


    public void Start()
    {
        initialPosition = transform.position;
        targetPosition = slideToRight ? initialPosition + Vector3.forward * slideDistance : initialPosition - Vector3.forward * slideDistance;

    }

    public void Update()
    {
        {
            // Move the door smoothly towards the target position
            if (isSliding)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);

                // Once the door reaches its target position, stop sliding
                if (transform.position == targetPosition)
                {
                    isSliding = false;
                }
            }
        }
    }

    public void Interact()
    {
        if(CompareTag("ElevatorDoor"))
        {
            {
                // If the door is not already sliding, start the sliding action
                if (!isSliding)
                {
                    isSliding = true;
                    if (isOpen)
                    {
                        // If the door is open, close it by sliding it back to its original position
                        targetPosition = initialPosition;
                    }
                    else
                    {
                        // Otherwise, slide the door to the target position (open it)
                        targetPosition = slideToRight ? initialPosition + Vector3.forward * slideDistance : initialPosition - Vector3.forward * slideDistance;
                    }
                    isOpen = !isOpen;  // Toggle the door state (open or closed)
                }
            }
        }
    }
}
