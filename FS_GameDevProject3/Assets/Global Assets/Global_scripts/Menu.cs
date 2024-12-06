using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{

/*--------------------------------------- SERIALIZED */

    [SerializeField] GameObject[] _buttons;


    public void DisableButtons()
    {
        foreach(GameObject button in _buttons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    public void EnableButtons()
    {
        foreach(GameObject button in _buttons)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }


}
