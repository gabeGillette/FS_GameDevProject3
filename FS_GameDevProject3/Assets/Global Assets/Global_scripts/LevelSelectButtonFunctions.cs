using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{

    /// <summary>
    /// GoToScene
    /// Enters a scene at id.
    /// </summary>
    /// <param name="id">The scene index</param>
    public void GoToScene(int id)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(id);
    }


    public void OpenOptionsMenu()
    {
        //
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
            GameManager.Instance.SavePlayerData(player);
        }

        ////curPlayerController = GameObject.FindGameObjectWithTag("Player");
        //GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        //playerController player = playerObject.GetComponent<playerController>();

        //GameManager.Instance.SavePlayerData(player);
    }
    public void load()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        playerController player = playerObject.GetComponent<playerController>();

        GameManager.Instance.LoadPlayerData(player);

        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex); // Reload the current scene

        // After the scene is reloaded, apply the saved data
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to sceneLoaded event
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When the scene is loaded, find the player and apply the saved data
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerController player = playerObject.GetComponent<playerController>();
            GameManager.Instance.LoadPlayerData(player); // Apply the saved player data
            Debug.Log("Player data loaded successfully.");
        }

        // Unsubscribe from the event to prevent multiple calls to this function
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
