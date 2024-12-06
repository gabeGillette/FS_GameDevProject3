using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.UIElements;
using TMPro;

public class MenuManager : MonoBehaviour
{


    [SerializeField] GameObject _modalPopup;
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject[] _allMenus;

    [SerializeField] UnityEngine.UI.Button _modalYes;
    [SerializeField] UnityEngine.UI.Button _modalNo;
    [SerializeField] TMP_Text _modalHeader;
    [SerializeField] TMP_Text _modalSubtitle;

    [SerializeField] AudioClip _hover;
    [SerializeField] AudioClip _accept;
    [SerializeField] AudioClip _deny;
    [SerializeField] AudioClip _next;
    [SerializeField] AudioClip _gunShot;

    [SerializeField] AudioSource _source;



    

    public enum MENU {PAUSE, SAVE, LOAD, MAIN, LEVEL_SELECT}

    private Dictionary <string, AudioClip> _menuSounds;
    private Dictionary <string, UnityAction> _buttonActions;


    // Start is called before the first frame update
    void Start()
    {
        _menuSounds = new Dictionary<string, AudioClip>(){{"hover", _hover}, {"accept", _accept}, {"deny", _deny}, {"next", _next}, {"gunshot", _gunShot}};
        
        _buttonActions = new Dictionary<string, UnityAction>();
        _buttonActions.Add("new", () => Debug.Log("new game"));
        
        _buttonActions.Add("load", () => Debug.Log("load game"));

        _buttonActions.Add("options", () => Debug.Log("options menu"));

        _buttonActions.Add("help", () => Debug.Log("help menu"));

        _buttonActions.Add("credits", () => Debug.Log("credits menu"));

        _buttonActions.Add("quit", () => {
            DisplayModal("Are you sure?", "Do you really want to quit?", () => Debug.Log("yes"), () => Debug.Log("no"));
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DisplayModal(string header, string subtitle, UnityAction YesAction, UnityAction NoAction)
    {
        DisableAllMenus();
        _modalPopup.gameObject.SetActive(true);
        EnableMenu(_modalPopup);
        _modalHeader.text = header;
        _modalSubtitle.text = subtitle;

        _modalYes.onClick.AddListener(YesAction);
        _modalNo.onClick.AddListener(NoAction);

    }


    public void DoButtonAction(string action)
    {
        _buttonActions[action]();
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




    public void PlaySoundEffect(string sound)
    {
        _source.PlayOneShot(_menuSounds[sound]);
    }
        

}
