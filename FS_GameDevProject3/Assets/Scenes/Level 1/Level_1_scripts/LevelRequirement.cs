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
    Basement,
}


public class LevelRequirement : MonoBehaviour, IInteractable
{
    public bool hasKey = false;  // Player state: does the player have the key?
    public TextMeshProUGUI messageText;  // Reference to the message UI element
    public float displayDuration = 2f;  // Duration the message will be displayed
    public TextMeshProUGUI questText;

    private currentSceneSelected currentScene;  // Current scene tracker
    private string[] sceneNames = {
        "Level 1",   // Scene for Level1
        "Level 2",   // Scene for Level2
        "Level 3",   // Scene for Level3
        "Basement"   // Scene for Basement
    };

    void Start()
    {
        messageText = GameObject.Find("Messages").GetComponent<TextMeshProUGUI>();
        questText = GameObject.Find("QuestTracker").GetComponent<TextMeshProUGUI>();

        
        messageText.text = "";  // Clear the text initially
        initializeCurrentScene();
        messageText.gameObject.SetActive(false);  // Ensure message text is hidden at the start
    }

    public void Interact()
    {
        if (hasKey)
        {
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
            case "Basement":
                currentScene = currentSceneSelected.Basement;
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
        int currentSceneIndex = (int)currentScene;  // Get the current scene's enum index
        int nextIndex = (currentSceneIndex + 1) % sceneNames.Length; // Get the next scene's index
        currentScene = (currentSceneSelected)nextIndex; // Set the current scene to the next one in the enum

        string nextSceneName = sceneNames[nextIndex];  // Get the scene name from the array using the next index

        Debug.Log("Next Scene: " + nextSceneName);  // Log the scene name to the console

        // Reset hasKey to false before transitioning to the next scene
        hasKey = false;

        questText.text = "";

        // Load the next scene by its name
        SceneManager.LoadScene(nextSceneName);
    }
}
