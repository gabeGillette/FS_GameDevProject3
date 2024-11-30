using UnityEngine;

public class Locker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player enters
        {
            other.gameObject.layer = LayerMask.NameToLayer("HiddenPlayer"); // Change layer
            Debug.Log("Player is now hidden in the locker.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player exits
        {
            other.gameObject.layer = LayerMask.NameToLayer("Player"); // Restore original layer
            Debug.Log("Player is now visible.");
        }
    }
}