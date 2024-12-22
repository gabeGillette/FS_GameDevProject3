using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    private void Awake()
    {
   //     DontDestroyOnLoad(gameObject); // Keep this object across scenes
    }

    /// <summary>
    /// GoToScene
    /// Enters a scene at id.
    /// </summary>
    /// <param name="id">The scene index</param>
    //public void GoToScene(int id)
    //{
    //    SceneManager.LoadScene(id);
    //}

    public void OpenOptionsMenu()
    {
        // Implement the functionality for opening the options menu
    }

    public void resume()
    {
        GameManager.Instance.UnpauseGame();
    }

    public void save()
    {
        // Find the player GameObject
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            // Get the playerController component
            playerController player = playerObject.GetComponent<playerController>();
            if (player != null)
            {
                // Call the SavePlayerData method from the GameManager
                GameManager.Instance.SavePlayerData(player);
                Debug.Log("Player data saved successfully.");
            }
            else
            {
                // Log error if playerController component is not found
                Debug.LogError("playerController component not found on player object!");
            }
        }
        else
        {
            // Log error if player GameObject is not found
            Debug.LogError("Player object not found!");
        }
    }

    public void load()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerController player = playerObject.GetComponent<playerController>();

        // Start the loading process asynchronously
        GameManager.Instance.LoadPlayerData(player);
    }

   
    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    // When the scene is loaded, find the player and apply the saved data
    //    GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
    //    if (playerObject != null)
    //    {
    //        playerController player = playerObject.GetComponent<playerController>();
    //        if (player != null)
    //        {
    //            GameManager.Instance.LoadPlayerData(player); // Apply the saved player data
    //            Debug.Log("Player data loaded successfully.");
    //        }
    //        else
    //        {
    //            Debug.LogError("PlayerController component not found on player object!");
    //        }
    //    }

    //    // Unsubscribe from the event to prevent multiple calls to this function
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}
}
