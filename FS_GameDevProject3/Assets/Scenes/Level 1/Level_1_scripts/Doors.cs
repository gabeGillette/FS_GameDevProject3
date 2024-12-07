using UnityEngine;

public class RotateAndMoveObject : MonoBehaviour, IInteractable
{
    public float rotationSpeed = 90f; // Speed at which the lever rotates
    public float rotationThreshold = 25f; // The degree amount after which the lever stops rotating
    public Vector3 targetPosition = new Vector3(10f, 0f, 0f); // Target position for the "MoveableBookShelf"
    public float moveSpeed = 2f; // Speed at which the bookshelf moves

    private bool isRotating = false; // Track if the lever is rotating
    private bool hasMoved = false; // Track if the bookshelf has already been moved
    private GameObject bookshelf; // Reference to the object to be moved (with the "MoveableBookShelf" tag)

    private float initialRotation; // Store the initial rotation of the lever
    private float currentRotation = 0f; // Store the current rotation of the lever

    // Implement Interact() method from IInteractable interface
    public void Interact()
    {
        if (!isRotating)
        {
            initialRotation = transform.rotation.eulerAngles.x; // Store the initial rotation of the lever
            currentRotation = 0f; // Reset the current rotation
            isRotating = true; // Start rotating the lever
        }

        // If the bookshelf hasn't been moved yet, move it
        if (!hasMoved)
        {
            bookshelf = GameObject.FindGameObjectWithTag("MoveableBookShelf"); // Find the bookshelf in the scene
            if (bookshelf != null)
            {
                StartCoroutine(MoveBookshelf()); // Start moving the bookshelf when the lever is pulled
            }
        }
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

    private System.Collections.IEnumerator MoveBookshelf()
    {
        // Move the bookshelf smoothly to the target position
        Vector3 initialPosition = bookshelf.transform.position;
        float journeyLength = Vector3.Distance(initialPosition, targetPosition);
        float startTime = Time.time;

        while (Vector3.Distance(bookshelf.transform.position, targetPosition) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            bookshelf.transform.position = Vector3.Lerp(initialPosition, targetPosition, fractionOfJourney);

            yield return null; // Wait until the next frame
        }

        // Ensure the bookshelf ends exactly at the target position
        bookshelf.transform.position = targetPosition;
        hasMoved = true; // Mark that the bookshelf has been moved
    }
}
