using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum currentSceneSelected
{
    Level1,
    Level2,
    Level3,
    Level4,
}


public class LevelRequirement : MonoBehaviour, IInteractable
{


    public bool hasKey = false;  // Player state: does the player have the key?
    public TextMeshProUGUI messageText;  // Reference to the message UI element
    public float displayDuration = 2f;  // Duration the message will be displayed
    public TextMeshProUGUI questText;
   // private GameObject _playerSpawn;

    private currentSceneSelected currentScene;  // Current scene tracker
    private string[] sceneNames = {
        "Level 1",   // Scene for Level1
        "Level 2",   // Scene for Level2
        "Level 3",   // Scene for Level3
        "Level 4"   // Scene for Basement
    };
    public LoadingScreen loadingScreen;
    public GameManager _gameManager;

  //  public playerController _playerScript;


    void Start()
    {
        GameObject loadingScreenObject = GameObject.FindGameObjectWithTag("LoadingScreen");
        loadingScreen = loadingScreenObject.GetComponent<LoadingScreen>();  // Assign the LoadingScreen component

        messageText = GameObject.Find("Messages").GetComponent<TextMeshProUGUI>();
        questText = GameObject.Find("QuestTracker").GetComponent<TextMeshProUGUI>();
      //  _playerSpawn = GameObject.FindWithTag("PlayerSpawn");

        messageText.text = "";  // Clear the text initially
        initializeCurrentScene();
        messageText.gameObject.SetActive(false);  // Ensure message text is hidden at the start
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

      //  loadingScreen.LoadSceneAsync(sceneName);
    }

    public void Interact()
    {
        if (hasKey)
        {
            //_gameManager.SavePlayerData(_playerScript);
            // Proceed to the next scene if the player has the key
            moveToNextScene();
        }
        else
        {
            // Show the message if the player doesn't have the key
            messageText.gameObject.SetActive(true);

            // Set the appropriate message based on the current scene
            initializeCurrentScene();

            // Start a coroutine to hide the message after the specified duration
            StartCoroutine(HideMessageAfterDelay(displayDuration));
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(delay);

        // Deactivate the message text
        messageText.gameObject.SetActive(false);
    }

    void initializeCurrentScene()
    {
        Scene currentActiveScene = SceneManager.GetActiveScene();

        // Set the message for the current scene
        switch (currentActiveScene.name)
        {
            case "Level 1":
                currentScene = currentSceneSelected.Level1;
                messageText.text = "Looks like you need a KeyCard to activate this.";
                questText.text += "\nFind KeyCard.\n";
                break;
            case "Level 2":
                currentScene = currentSceneSelected.Level2;
                messageText.text = "Looks like you need a Code to activate this.";
                questText.text += "\nFind the 4 digit code.\n";
                break;
            case "Level 3":
                currentScene = currentSceneSelected.Level3;
                messageText.text = "Looks like you need an Old Key to activate this.";
                questText.text += "\nFind Old Key.\n";
                break;
            case "Level 4":
                currentScene = currentSceneSelected.Level4;
                messageText.text = "The elevator is broken.";
                questText.text = "";
                questText.text = "ESCAPE!";
               
                break;
            default:
                Debug.LogWarning("Current scene not recognized.");
                break;
        }
    }

    void moveToNextScene()
    {
        // Get the current scene index (from enum)
        int currentSceneIndex = (int)currentScene;

        // Get the next scene index. If we're at the last scene, it will loop back to the first one.
        int nextIndex = (currentSceneIndex + 1) % sceneNames.Length;

        // Update the current scene variable with the next scene
        currentScene = (currentSceneSelected)nextIndex;

        // Get the name of the next scene from the sceneNames array
        string nextSceneName = sceneNames[nextIndex];

        // Log to the console for debugging
        Debug.Log("Current Scene: " + sceneNames[currentSceneIndex]);
        Debug.Log("Next Scene: " + nextSceneName);

        // Reset any game state before loading the next scene (e.g., resetting keys or objectives)
        hasKey = false; // Example of resetting the key state, if needed
        questText.text = ""; // Reset the quest text

        // Load the next scene by its name, not index
        ChangeScene(nextSceneName);
    }
}
