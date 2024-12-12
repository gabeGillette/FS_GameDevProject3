using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class questTracking : MonoBehaviour
{

    public TextMeshProUGUI questText;  // Reference to the message UI element

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
}
