using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen: MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loadingScreen;  // The loading screen UI
    public TextMeshProUGUI loadingText;  // TextMeshPro text (or use Text if you prefer)

    // This function will be called when you start loading a new scene
    public void LoadSceneAsync(string sceneName)
    {
        // Show the loading screen UI
        loadingScreen.SetActive(true);

        // Start the scene loading process in the background
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    // Coroutine to handle the scene loading process asynchronously
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Start loading the scene in the background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Don't let the scene activate until it's completely loaded
        asyncLoad.allowSceneActivation = false;

        // While the scene is loading, keep the loading screen active
        while (!asyncLoad.isDone)
        {
            // Here, you can add logic to update the loading message if you like
            if (loadingText != null)
            {
                loadingText.text = "Loading..."; // Display "Loading..." or any message
            }

            // When loading is almost done (90%), you can prompt the user to continue
            if (asyncLoad.progress >= 0.9f)
            {
                if (loadingText != null)
                {
                    loadingText.text = "Press any key to continue...";  // Update message
                }

                // Wait for user input before allowing scene activation
                if (Input.anyKeyDown)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
