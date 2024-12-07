using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  


public class playerCheckInteraction : MonoBehaviour
{
    public float interactionRange = 3f;  // The range at which the player can interact
    private IInteractable interactableObject;  // The object the player can interact with

    // UI elements to display the "Press E to interact" message
    public TextMeshProUGUI interactText;  // Drag and drop the Text component from the UI in the inspector

  
    void Start()
    {
        interactText = GameObject.Find("InteractText").GetComponent<TextMeshProUGUI>();  


        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);  // Make sure the text is initially hidden
        }
        
    }

    void Update()
    {
     

        // Check for nearby interactable objects
        CheckForInteractableObjects();

        // If an interactable object is within range, show the interaction prompt
        if (interactableObject != null)
        {
            if (interactText != null)
            {
                interactText.gameObject.SetActive(true);  // Show the text
                Debug.Log("Showing 'Press E to interact' message.");

            }

            // If the player presses "E", interact with the object
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactableObject.Interact();  // Call Interact() on the object
            }
        }
        else
        {
            if (interactText != null)
            {
                interactText.gameObject.SetActive(false);  // Hide the text if no interactable object is nearby
                Debug.Log("Hiding 'Press E to interact' message.");

            }
        }
    }

    void CheckForInteractableObjects()
    {
        // Cast a sphere to detect interactable objects within range
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
        interactableObject = null;  // Reset the interactable object

        foreach (var collider in colliders)
        {
            // Check if the collider has an IInteractable component
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactableObject = interactable;  // Set the interactable object
                break;  // We found an interactable object, no need to check further
            }
        }
    }
}
