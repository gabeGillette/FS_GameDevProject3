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
    [SerializeField] GraphicRaycaster _raycaster;
    [SerializeField] EventSystem _eventSystem;
    PointerEventData _pointerEventData;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _sfx_hover;

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


    public void OnHover()
    {
        _audioSource.PlayOneShot(_sfx_hover);
    }
}
