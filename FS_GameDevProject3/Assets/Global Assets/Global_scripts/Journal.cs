using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Journal : MonoBehaviour
{
    public bool journalone;
    public bool journaltwo;
    public bool journalthree;


    public TextMeshProUGUI firstJournal;  // Reference to the message UI element
    public TextMeshProUGUI secondJournal;
    public TextMeshProUGUI thirdJournal;

    public GameObject _mainPanel;

    public playerController _playerController;
    private bool isDisplayed;
    private float _timeScale;

    public GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _mainPanel.SetActive(false);
        _timeScale = 1.0f;
        _playerController = FindObjectOfType<playerController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Journal"))
        {
            isDisplayed = !isDisplayed;
            displayJournal();
        }
        ActivateJournalBasedOnLevel();
        Debug.Log(_gameManager.GetCurrentLevel());
    }

    void displayJournal()
    {
        //Turn off the quest display
        if (isDisplayed)
        {
            _mainPanel.SetActive(false);
            Time.timeScale = _timeScale;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            //firstJournal.gameObject.SetActive(false);
        }
        else
        {
            _mainPanel.SetActive(true);
            Time.timeScale = 0.0f;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            //questText.gameObject.SetActive(true);
        }

    }

    void ActivateJournalBasedOnLevel()
    {
        if (_gameManager == null) return; // Ensure GameManager is set

        int currentLevel = _gameManager.GetCurrentLevel(); // Get the current level from GameManager

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
