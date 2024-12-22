using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{


    [SerializeField] GameObject _modalPopup;
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _mainMenu;
  //  [SerializeField] GameObject _gameManager;

    [SerializeField] GameObject _optionsMenu;

    [SerializeField] GameObject _helpMenu;

    [SerializeField] GameObject _LoadMenu;
    [SerializeField] GameObject _storyBoard;
    [SerializeField] GameObject _credits;

    //[SerializeField] GameObject[] _allMenus;

    [SerializeField] UnityEngine.UI.Button _modalYes;
    [SerializeField] UnityEngine.UI.Button _modalNo;
    [SerializeField] TMP_Text _modalHeader;
    [SerializeField] TMP_Text _modalSubtitle;

    //[SerializeField] UnityEngine.UI.Button _Options_Apply;
    //[SerializeField] UnityEngine.UI.Button _Options_Default;
    //[SerializeField] UnityEngine.UI.Button _Options_Cancel;

    [SerializeField] AudioClip _hover;
    [SerializeField] AudioClip _accept;
    [SerializeField] AudioClip _deny;
    [SerializeField] AudioClip _next;
    [SerializeField] AudioClip _gunShot;

    [SerializeField] AudioSource _source;

    private bool _OptionsDirty = true;



    

    public enum MENU {PAUSE, SAVE, LOAD, MAIN, LEVEL_SELECT, OPTIONS}

    private Dictionary <string, AudioClip> _menuSounds;
    private Dictionary <string, UnityAction> _buttonActions;


    // Start is called before the first frame update
    void Start()
    {
        GameObject globalSaveObject = GameObject.Find("GlobalSaveChecker");


        _menuSounds = new Dictionary<string, AudioClip>(){{"hover", _hover}, {"accept", _accept}, {"deny", _deny}, {"next", _next}, {"gunshot", _gunShot}};
        
        _buttonActions = new Dictionary<string, UnityAction>();
        _buttonActions.Add("new", () => StartNewGame()); // Starts new game

        _buttonActions.Add("load", () =>
        {
            // _gameManager.LoadPlayerData();
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            if (currentLevel != SceneManager.GetActiveScene().buildIndex)
            {
                SceneManager.LoadScene(currentLevel); // Load the saved level, if necessary
                _helpMenu.gameObject.SetActive(false); // Disable the help menu

            }
            //   GlobalSaveChecker globalSaveChecker = globalSaveObject.GetComponent<GlobalSaveChecker>();
            //   globalSaveChecker._isLoad = true;
        });
       

        _buttonActions.Add("options", () => {
            _optionsMenu.gameObject.SetActive(true);
        });

        _buttonActions.Add("help", () => {
            _helpMenu.gameObject.SetActive(true);
        });

        _buttonActions.Add("story", () =>
        {
            _storyBoard.gameObject.SetActive(true);
        });
            
        _buttonActions.Add("credits", () =>
        {
            _credits.gameObject.SetActive(true);
        });

        _buttonActions.Add("quit", () => {
            DisplayModal("Are you sure?", "Do you really want to quit?", 
            () => QuitGame(), 
            () => CloseModal());
        });

        _buttonActions.Add("options_apply", () => Debug.Log("applied options"));
        _buttonActions.Add("options_default", () => {
            DisplayModal("Are you sure?", "This will reset your settings back to default values.", 
            () => CloseModal(), 
            () => CloseModal());
        });
        
        _buttonActions.Add("options_cancel", () => {
            if(_OptionsDirty){
                DisplayModal("Are you sure?", "You have unsaved changes", 
                () => {
                    CloseModal();
                    _optionsMenu.gameObject.SetActive(false);
                }, 
                () => CloseModal());
            }
            else
            {
                _optionsMenu.gameObject.SetActive(false);

            }
        });
        _buttonActions.Add("cancel", () =>
        {
            if (_helpMenu != null)
            {
                _helpMenu.gameObject.SetActive(false); // Disable the help menu
                _credits.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("_helpMenu is not assigned in the Inspector!");
            }
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DisplayModal(string header, string subtitle, UnityAction YesAction, UnityAction NoAction)
    {
        //DisableAllMenus();
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
        /*foreach(GameObject menu in _allMenus)
        {
            menu.gameObject.SetActive(false);
        }*/
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
        /*foreach(GameObject menu in _allMenus)
        {
            DisableMenu(menu);
        }*/
    }




    public void PlaySoundEffect(string sound)
    {
        _source.PlayOneShot(_menuSounds[sound]);
    }
        
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    public void CloseModal()
    {
        _modalPopup.SetActive(false);
    }
    public void StartNewGame()
    {
        // Load the first scene (you can replace with a specific scene name if necessary)
        SceneManager.LoadScene(1); // Loads the first scene in the build settings
    }

    //public void LoadGame()
    //{
    //    GameManager.Instance.LoadPlayerData();
    //}

    public void HowToPlay()
    {

    }
    public void BackToMainMenu()
    {
        _helpMenu.gameObject.SetActive(false);


    }

    
}
