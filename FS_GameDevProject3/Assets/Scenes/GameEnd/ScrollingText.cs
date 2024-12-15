using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollingText : MonoBehaviour
{
    public float scrollSpeed = 50f;  // Speed of scrolling
    public float resetHeight = -500f; // Height to reset to once the text scrolls past it
    public float fastScrollSpeed = 100f; // Speed when a key is held down

    private RectTransform textRectTransform;

    void Start()
    {
        textRectTransform = GetComponent<RectTransform>(); // Get the RectTransform of the text
    }

    void Update()
    {
        // Check if any key is being held down
        if (Input.anyKey)
        {
            // If any key is pressed, use the fast scroll speed
            textRectTransform.anchoredPosition += new Vector2(0, fastScrollSpeed * Time.deltaTime);
        }
        else
        {
            // If no key is pressed, use the normal scroll speed
            textRectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        }
    }
    public void GoToMainMenu()
    {
        // Load the main menu scene (replace "MainMenu" with your actual main menu scene name)
        SceneManager.LoadScene(0);
    }
}
