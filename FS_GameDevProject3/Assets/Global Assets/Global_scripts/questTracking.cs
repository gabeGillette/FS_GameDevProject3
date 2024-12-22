using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class questTracking : MonoBehaviour
{

    public TextMeshProUGUI questText;  // Reference to the message UI element
    public GameManager _gameManager;


    private bool isDisplayed;

    // Start is called before the first frame update
    void Start()
    {
      //  questText.text = "Find what's causing the disturbance in the Old Mansion.";
      
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Quest"))
        {
            isDisplayed = !isDisplayed;
            displayQuest();
        }
        JournalReminder();
    }

    void displayQuest()
    {
            //Turn off the quest display
            if (isDisplayed)
            {
                questText.gameObject.SetActive(false);
            }
            else 
            {
                questText.gameObject.SetActive(true);
            }

    }

    void JournalReminder()
    {
        if(_gameManager._evidenceCollected >= _gameManager._evidenceTotal)
        {
            _gameManager._UIQuest.text = "Get to the Elevator and Get to the next Level";
            _gameManager.UpdateUI();
        }

    }

    
}
