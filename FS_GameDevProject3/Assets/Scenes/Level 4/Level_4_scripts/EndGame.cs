using UnityEngine;
using UnityEngine.SceneManagement;  // Import the SceneManager class

public class SceneChanger : MonoBehaviour
{
    // This method will be called when a collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided with the trigger is tagged "Player" (or whatever tag you want)
        if (other.CompareTag("Player"))
        {
            // Load scene 5 (you can also use Scene name like "Scene5" if you have a specific name)
            SceneManager.LoadScene(5);  // Change to scene index 5
        }
    }
}
