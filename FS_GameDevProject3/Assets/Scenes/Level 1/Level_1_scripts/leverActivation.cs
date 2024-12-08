using UnityEngine;

public class RotateOnActivate : MonoBehaviour, IInteractable
{
    public float rotationSpeed = 90f; // Speed at which the lever rotates
    public float rotationThreshold = 25f; // The degree amount after which the lever stops rotating
    public float targetXPosition = 0.1f; // Target X position for the "MoveableBookShelf" (unused in this case)

    private bool isRotating = false; // Track if the lever is rotating
    private bool hasDestroyed = false; // Track if the bookshelf has already been destroyed
    private GameObject bookshelf; // Reference to the object to be moved (with the "MoveableBookShelf" tag)

    private float initialRotation; // Store the initial rotation of the lever
    private float currentRotation = 0f; // Store the current rotation of the lever

    private bool canInteract = true; // Track if the lever can still be interacted with

    public AudioClip leverSound;


    // Implement Interact() method from IInteractable interface
    public void Interact()
    {
        // If the lever has already been interacted with, do nothing
        if (!canInteract) return;

        // If the lever hasn't been interacted with yet, start the rotation and destruction process
        if (!isRotating)
        {
            AudioSource.PlayClipAtPoint(leverSound, transform.position);

            initialRotation = transform.rotation.eulerAngles.x; // Store the initial rotation of the lever
            currentRotation = 0f; // Reset the current rotation
            isRotating = true; // Start rotating the lever
        }

        // If the bookshelf hasn't been destroyed yet, destroy it
        if (!hasDestroyed)
        {
            bookshelf = GameObject.FindGameObjectWithTag("MoveAbleBookCase"); // Find the bookshelf in the scene
            if (bookshelf != null)
            {
                Destroy(bookshelf); // Destroy the bookshelf when the lever is pulled
                hasDestroyed = true; // Mark that the bookshelf has been destroyed
            }
        }

        // Disable further interactions with the lever
        canInteract = false; // Set to false to prevent further interactions
    }

    void Update()
    {
        // If the lever is rotating, rotate it
        if (isRotating)
        {
            RotateLever();
        }
    }

    private void RotateLever()
    {
        // Rotate the lever around the X-axis
        float rotationAmount = rotationSpeed * Time.deltaTime;
        transform.Rotate(rotationAmount, 0f, 0f); // Apply rotation to the lever

        currentRotation += rotationAmount;

        // Stop rotating once we reach the rotation threshold
        if (Mathf.Abs(currentRotation) >= rotationThreshold)
        {
            isRotating = false;
            currentRotation = rotationThreshold; // Ensure it doesn't go beyond the threshold
        }
    }
}
