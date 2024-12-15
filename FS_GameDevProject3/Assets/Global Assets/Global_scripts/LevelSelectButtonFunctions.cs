using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Keep this object across scenes
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
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerController player = playerObject.GetComponent<playerController>();
            if (player != null)
            {
                GameManager.Instance.SavePlayerData(player);
            }
            else
            {
                Debug.LogError("PlayerController component not found on player object!");
            }
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    public void load()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerController player = playerObject.GetComponent<playerController>();
            if (player != null)
            {
                GameManager.Instance.LoadPlayerData(player); // Load player data first
            }
            else
            {
                Debug.LogError("PlayerController component not found on player object!");
            }
        }
        else
        {
            Debug.LogError("Player object not found!");
        }

        // Optionally reload the current scene if necessary (if it's part of the load function)
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);

        // Subscribe to sceneLoaded event to handle player data loading once the scene is loaded
      //  SceneManager.sceneLoaded += OnSceneLoaded;
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
