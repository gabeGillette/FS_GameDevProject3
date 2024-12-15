using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    public bool journalone;
    public bool journaltwo;
    public bool journalthree;

    public TextMeshProUGUI firstJournal;  // Reference to the first journal UI element
    public TextMeshProUGUI secondJournal; // Reference to the second journal UI element
    public TextMeshProUGUI thirdJournal;  // Reference to the third journal UI element

    public GameObject _mainPanel;  // The panel that contains the journal UI

    public playerController _playerController;  // Reference to the player controller (if needed)
    private bool isDisplayed;  // Whether the journal is currently displayed or not
    private float _timeScale;  // The time scale used when the journal is opened/closed

    public GameManager _gameManager;  // Reference to the GameManager to get the current level
    public Button _journalButton;  // Reference to the UI Button that opens/closes the journal

    // Start is called before the first frame update
    void Start()
    {
        _mainPanel.SetActive(false);  // Hide the journal panel initially
        _timeScale = 1.0f;  // Normal time scale (game not paused)
        _playerController = FindObjectOfType<playerController>();  // Find player controller (if needed)

        // Add listener for the button click
        if (_journalButton != null)
        {
            _journalButton.onClick.AddListener(ToggleJournalDisplay);  // Toggle the journal display on button click
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Listen for keyboard input to toggle the journal display
        if (Input.GetButtonDown("Journal"))
        {
            ToggleJournalDisplay();
        }

        // Activate journal based on the current level
        ActivateJournalBasedOnLevel();
    }

    // Toggles the journal display between open/close
    void ToggleJournalDisplay()
    {
        isDisplayed = !isDisplayed;  // Toggle the display flag
        displayJournal();  // Update the journal UI visibility
    }

    // Updates the journal UI visibility based on the current state
    void displayJournal()
    {
        // If the journal is open, hide the panel and resume the game
        if (isDisplayed)
        {
            _mainPanel.SetActive(true);  // Show the journal panel
            Time.timeScale = 0.0f;  // Pause the game (stop time)
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;  // Unlock the cursor for interaction
            UnityEngine.Cursor.visible = true;  // Show the cursor
        }
        else
        {
            _mainPanel.SetActive(false);  // Hide the journal panel
            Time.timeScale = _timeScale;  // Restore game time to normal
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;  // Lock the cursor back to the center of the screen
            UnityEngine.Cursor.visible = false;  // Hide the cursor
        }
    }

    // Activates the correct journal entries based on the player's current level
    void ActivateJournalBasedOnLevel()
    {
        if (_gameManager == null) return;  // Ensure GameManager is set

        int currentLevel = _gameManager.GetCurrentLevel();  // Get the current level from GameManager

        // Show the appropriate journal based on the current level
        if (currentLevel == 2)
        {
            journalone = true;
            journaltwo = false;
            journalthree = false;
        }
        else if (currentLevel == 3)
        {
            journalone = true;
            journaltwo = true;
            journalthree = false;
        }
        else if (currentLevel == 4)
        {
            journalone = true;
            journaltwo = true;
            journalthree = true;
        }

        // Activate the journals based on their respective booleans
        firstJournal.gameObject.SetActive(journalone);
        secondJournal.gameObject.SetActive(journaltwo);
        thirdJournal.gameObject.SetActive(journalthree);
    }
}
