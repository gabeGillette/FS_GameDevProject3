using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
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
    public bool hasKey = false;
    private string sceneToLoad;

    public TextMeshProUGUI messageText;  // Drag and drop the Text component from the UI in the inspector
    public float displayDuration = 2f;  // Duration the message will be displayed

    private currentSceneSelected currentScene;  // Current scene tracker
    private string messageForAccess;


    private string[] sceneNames = {
        "Level 1",   // Scene for Level1
        "Level 2",   // Scene for Level2
        "Level 3",   // Scene for Level3
        "Basement"   // Scene for Basement
    };
    // Start is called before the first frame update
    void Start()
    {
        
        messageText = GameObject.Find("Messages").GetComponent<TextMeshProUGUI>();

        messageText.gameObject.SetActive(false);  // Ensure message text is hidden at the start


        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact()
    {

        if (hasKey)
        {
            moveToNextScene();
            
        }
        else
        {
            if (currentScene == currentSceneSelected.Level1)
            {
                // Set the message text
                messageText.text = "Looks like you need a KeyCard to activate this.";

                // Activate the message text
                messageText.gameObject.SetActive(true);

                // Start a coroutine to hide the message after a delay
                StartCoroutine(HideMessageAfterDelay(displayDuration));
            }
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
        switch (currentActiveScene.name)
        {
            case "Level 1":
                currentScene = currentSceneSelected.Level1;
                break;
            case "Level 2":
                currentScene = currentSceneSelected.Level2;
                break;
            case "Level 3":
                currentScene = currentSceneSelected.Level3;
                break;
            case "Basement":
                currentScene = currentSceneSelected.Basement;
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
        hasKey = false;
        // Load the next scene by its name
        SceneManager.LoadScene(nextSceneName);
    }
    void messages(int messageNumber)
    {
        switch(messageNumber)
        {

            case 0: //Access to the 2nd Floor
                messageForAccess = "Looks like you need a KeyCard to activate this.";
                break;
            case 1: //Access to the 3rd Floor
                messageForAccess = "Looks like you need a Code to activate this.";
                break;
            case 2: //Access to the basement
                messageForAccess = "Looks like you need a Old Key to activate this.";
                break;
        }
    }
}
