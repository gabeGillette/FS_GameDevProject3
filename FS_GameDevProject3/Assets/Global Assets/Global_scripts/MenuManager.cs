using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class MenuManager : MonoBehaviour
{


    [SerializeField] GameObject _confirmMenu;
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject[] _allMenus;

    [SerializeField] Button _confirmYes;
    [SerializeField] Button _confirmNo;

    [SerializeField] AudioClip _hover;
    [SerializeField] AudioClip _accept;
    [SerializeField] AudioClip _deny;
    [SerializeField] AudioClip _next;
    [SerializeField] AudioClip _gunShot;

    [SerializeField] AudioSource _source;

    

    public enum MENU {PAUSE, SAVE, LOAD, MAIN, LEVEL_SELECT}
    public enum BUTTON_FUNCTION {RESUME, QUIT, SAVE, LOAD, NEW_GAME, OPTIONS}


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ConfirmDestructive(UnityAction YesAction, UnityAction NoAction)
    {
        DisableAllMenus();
        _confirmMenu.gameObject.SetActive(true);
        EnableMenu(_confirmMenu);

        _confirmYes.onClick.AddListener(YesAction);
        _confirmNo.onClick.AddListener(NoAction);

    }




    public void KillAllMenus()
    {
        foreach(GameObject menu in _allMenus)
        {
            menu.gameObject.SetActive(false);
        }
    }

    public void DisableMenu(GameObject menu)
    {
        menu.gameObject.GetComponent<Menu>().DisableButtons();
    }

    public void EnableMenu(GameObject menu)
    {
        menu.gameObject.GetComponent<Menu>().EnableButtons();
    }

    public void DisableAllMenus()
    {
        foreach(GameObject menu in _allMenus)
        {
            DisableMenu(menu);
        }
    }


    public void ButtonFunction(BUTTON_FUNCTION function)
    {
        switch(function)
        {
            case BUTTON_FUNCTION.RESUME:
                KillAllMenus();
                GameManager.Instance.UnpauseGame();
                break;

            case BUTTON_FUNCTION.QUIT:
                DisableAllMenus();


                break;
            case BUTTON_FUNCTION.SAVE:
                break;
            case BUTTON_FUNCTION.LOAD:
                break;
            case BUTTON_FUNCTION.NEW_GAME:
                break;
            case BUTTON_FUNCTION.OPTIONS:
                break;
            
        }
    }

    public void PlayHover()
    {
        _source.PlayOneShot(_hover);
    }

}
