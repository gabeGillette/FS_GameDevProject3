using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikeTrap : MonoBehaviour
{
    public float upDistance = 2f; // How far the spikes move up
    public float moveSpeed = 2f; // Speed at which spikes move
    public float timerInterval = 1f; // Time between up and down movements

    public bool shouldMove;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMovingUp = true;
    

    void Start()
    {
        startPosition = transform.position; // Initial position of the spikes
        targetPosition = startPosition + Vector3.up * upDistance; // Target position when spikes go up

        // Start the movement cycle with the timer
        if (shouldMove)
        {
            InvokeRepeating("ToggleSpikeMovement", 0f, timerInterval);
        }
    }

    void ToggleSpikeMovement()
    {
        if (isMovingUp)
        {
            StartCoroutine(MoveSpike(targetPosition)); // Move up
        }
        else
        {
            StartCoroutine(MoveSpike(startPosition)); // Move back down
        }

        isMovingUp = !isMovingUp; // Toggle between up and down
    }

    System.Collections.IEnumerator MoveSpike(Vector3 target)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;

        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(initialPosition, target, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // Ensure it reaches the exact position
    }

   
}
