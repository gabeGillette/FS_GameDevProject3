using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
//using UnityEditor.ProBuilder;

public class GameManager : MonoBehaviour
{
    /*------------------------------------------- SERIALIZED */

    [SerializeField] GameObject _playerPrefab;
    [SerializeField] Image _reticle;

    [SerializeField] TMP_Text _UITopLeft;
    [SerializeField] TMP_Text _UITopRight;
    [SerializeField] TMP_Text _UIMessages;
    [SerializeField] public TMP_Text _UIQuest;

    [Header("-----Menus-----")]
    [SerializeField] GameObject _menuActive;
    [SerializeField] GameObject _menuPause;
    private GameObject _reticleObject;

    [SerializeField] [Range(0, 20)] float _messageDuration;

    [SerializeField] TextAsset _versionFile;


    /*------------------------------------------ PRIVATE MEMBERS */

    static private GameManager _instance;

    public GameObject _player;
    private playerController _playerScript;
    private List<GameObject> _evidenceList;
    public GameObject _playerSpawn;
    public GlobalSaveChecker _saveChecker;

    public int _evidenceTotal;
    public int _evidenceCollected;

    public int _currentLevel;
    public TextMeshProUGUI messageText;  // Reference to the message UI element

    private List<string> _messageList = new List<string>();

    private bool _isPaused = false;
    private bool isDisplayed;
    private float _timeScale;

    // Store player data between scenes
    private int savedHP;
    private int savedGunIndex;
    private int savedAmmoCur;
    private int savedAmmoRes;

   // private bool remindAboutJournal;


    /*------------------------------------------ PUBLIC ACCESSORS */

    public static GameManager Instance => _instance;
    public GameObject Player => _player;
    public playerController PlayerScript => _playerScript;

    /*--------------------------------------------- PRIVATE METHODS */

    // Start is called before the first frame update
    void Awake()
    {
     //    GameObject globalSaveObject = GameObject.Find("GlobalSaveChecker");
      //  GlobalSaveChecker globalSaveChecker = globalSaveObject.GetComponent<GlobalSaveChecker>();

        _reticleObject = GameObject.FindGameObjectWithTag("Reticle");
        _isPaused = false;
        _timeScale = 1.0f;
        messageText = GameObject.Find("Messages").GetComponent<TextMeshProUGUI>();
     //   GameObject globalSaveObject = GameObject.Find("GlobalSaveObject");
        _currentLevel = SceneManager.GetActiveScene().buildIndex;

        // find the playerspawner
        _playerSpawn = GameObject.FindWithTag("PlayerSpawn");
       // _player = GameObject.FindWithTag("Player");

        if (_playerSpawn != null)
        {
            if (_player == null)
            {
                
                    RespawnPlayer(_playerSpawn.transform);
                

              //  SetPlayerReference();
            }
        }

        // find all the evidence interactables
        _evidenceList = new List<GameObject>();
        _evidenceList.AddRange(GameObject.FindGameObjectsWithTag("Evidence"));

        _evidenceCollected = 0;
        _evidenceTotal = _evidenceList.Count;

        if (_instance == null)
        {
            _instance = this;
         //   DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateUI();
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_menuActive == null)
            {
                PauseGame();
               
            }
            else if (_menuActive == _menuPause)
            {
                UnpauseGame();
            }

        }
        _player = GameObject.FindWithTag("Player");

       

    }

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }
    // Add a method to store player data
    public void SavePlayerData(playerController player)
    {
        // Save player health
        PlayerPrefs.SetInt("PlayerHealth", player.HPCurrent);

        // Save selected gun index
        PlayerPrefs.SetInt("PlayerGunIndex", player._selectedGun);

        // Save current ammo in selected gun
        PlayerPrefs.SetInt("PlayerAmmoCur", player.SelectedGun.ammoCur);

        // Save ammo reserves
        PlayerPrefs.SetInt("PlayerAmmoRes", player.SelectedGun.ammoRes);

        // Save Evidence Collected
        PlayerPrefs.SetInt("TotalEvidenceCollected", _evidenceCollected);


      //  Instance._playerSpawn.transform.position = transform.position;
        // Save player position
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", player.transform.position.z);

        // Save current level index (if needed for scene tracking)
        PlayerPrefs.SetInt("CurrentLevel", _currentLevel);

        // Save all data to disk
        PlayerPrefs.Save();

        // Debug log to check if saving worked (optional)
        Debug.Log("Player Data Saved: Health = " + player.HPCurrent + ", Gun Index = " + player._selectedGun);
    }

    // Add a method to load player data
    public void LoadPlayerData(playerController player)
    {
        // Load saved player data from PlayerPrefs
        int loadedHP = PlayerPrefs.GetInt("PlayerHealth", player.HPCurrent); // Default to current health if no saved value
        int loadedGunIndex = PlayerPrefs.GetInt("PlayerGunIndex", player._selectedGun); // Default to current gun index if no saved value
        int loadedAmmoCur = PlayerPrefs.GetInt("PlayerAmmoCur", player.SelectedGun.ammoCur); // Default to current ammo if no saved value
        int loadedAmmoRes = PlayerPrefs.GetInt("PlayerAmmoRes", player.SelectedGun.ammoRes); // Default to current ammo reserve if no saved value

        // Load player position
        //float loadedPosX = PlayerPrefs.GetFloat("PlayerPosX", player.transform.position.x); // Default to current position if no saved value
        //float loadedPosY = PlayerPrefs.GetFloat("PlayerPosY", player.transform.position.y);
        //float loadedPosZ = PlayerPrefs.GetFloat("PlayerPosZ", player.transform.position.z);

        float posX = PlayerPrefs.GetFloat("PlayerPosX", _playerSpawn.transform.position.x);  // Default to spawn position if not saved
        float posY = PlayerPrefs.GetFloat("PlayerPosY", _playerSpawn.transform.position.y);
        float posZ = PlayerPrefs.GetFloat("PlayerPosZ", _playerSpawn.transform.position.z);
        //RespawnPlayer(_playerSpawn.transform);


        // Debug logs to check loaded data (optional)
        Debug.Log("Loading Player Data:");
        Debug.Log("Loaded Health: " + loadedHP);
        Debug.Log("Loaded Gun Index: " + loadedGunIndex);
        Debug.Log("Loaded Ammo Current: " + loadedAmmoCur);
        Debug.Log("Loaded Ammo Reserve: " + loadedAmmoRes);
        //Debug.Log("Loaded Position: " + new Vector3(loadedPosX, loadedPosY, loadedPosZ));

        // Apply loaded data to the player
        player.SetHealth(loadedHP); // Apply the loaded health
        player.SetGun(loadedGunIndex); // Apply the loaded gun index
        player.SetAmmo(loadedAmmoCur, loadedAmmoRes); // Apply the loaded ammo counts

       

        // Optionally, you may want to reload the scene to match the loaded level data
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        // Avoid scene reload while applying player data
        if (currentLevel != SceneManager.GetActiveScene().buildIndex)
        {
            // Asynchronously load the new scene
            StartCoroutine(LoadSceneAsync(currentLevel));
        }
        RespawnPlayer(_playerSpawn.transform);

        // Set player position
        //   player.transform.position = new Vector3(loadedPosX, loadedPosY, loadedPosZ);
        UpdateUI();
        // Debug log for confirming the data was applied


        Debug.Log("Player data loaded successfully.");
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        // Don't let the scene activate until everything is loaded
        asyncLoad.allowSceneActivation = false;

        // Wait until the scene has fully loaded
        while (!asyncLoad.isDone)
        {
            // Optionally, you could show a progress bar here, or log the loading progress
            // Debug.Log("Loading Progress: " + asyncLoad.progress);

            // The scene is fully loaded when the `progress` is at 0.9f
            if (asyncLoad.progress >= 0.9f)
            {
                // Activate the scene now that it's fully loaded
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Scene loaded and activated successfully.");
    }

    public void CollectEvidence()
    {
        _evidenceCollected++;
        UpdateUI();
    }

    public void GetPickup(GameObject pickup)
    {
        // TODO display ui message

        // just logging for now
      //  Debug.Log("Collected " + pickup.name);

        // get component
        pickup.TryGetComponent<IPickup>(out IPickup pickupScript);

        // fire event
        if (pickupScript != null)
        {
            pickupScript.CollectEvent();
        }
        else 
        {
            Debug.LogError("Pickup has no script!");
        }

        // TODO play sound

        // destroy the object
        Destroy(pickup);
    }


    public void RespawnPlayer(Transform spawnPoint)
    {
       
        if (_player != null)
        {
            // despawn the player if it's already in the scene
            Destroy(_player);
        }

        // instantiate a new player
        Instantiate(_playerPrefab, spawnPoint.position, spawnPoint.rotation);


        SetPlayerReference();
       // _playerScript.restoreHealth(100);
        UpdateUI();
       // LoadPlayerData(_player.GetComponent<playerController>());

    }



    private void SetPlayerReference()
    {
        // Find the player
        _player = GameObject.FindWithTag("Player");

        if (_player == null)
        {
           // Debug.LogError("Player does not exist!");
        }
        else
        {
         //   Debug.Log("Player found successfully!");
        }

        _player.TryGetComponent<playerController>(out _playerScript);
        if (_playerScript == null)
        {
          //  Debug.LogError("Player is missing PlayerController!");
        }
        else
        {
          //  Debug.Log("PlayerController script assigned successfully!");
        }
    }


    public void UpdateUI()
    {
        _UITopLeft.text = (
       $"Ammo: {PlayerScript.SelectedGun.ammoCur} / {PlayerScript.SelectedGun.ammoRes}\n" +
       $"Evidence: {_evidenceCollected}/{_evidenceTotal}\n");
      // $"Monsters Spawned: {0}/{0}\n" +
      // $"Monsters Killed: {0}/{0}");

        _UITopRight.text = ($"Level: {_currentLevel}\n" +
            $"Version: {_versionFile.text}");

        string fullMessage = "";
        string fullQuest = "Investigate the Strange Events in the Mansion";

        foreach(string message in _messageList)
        {
            fullMessage += message + "\n";
        }

        _UIMessages.text = fullMessage;
        _UIQuest.text = fullQuest;
    }

    public void PauseGame()
    {
        _isPaused = true;
        _reticleObject.SetActive(false);
        _menuActive = _menuPause;
        _menuActive.SetActive(true);
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UnpauseGame()
    {
        _isPaused = false;
        _reticleObject.SetActive(true);
        _menuActive.SetActive(false);
        _menuActive = null;
        Time.timeScale = _timeScale;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method is called after a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the PlayerSpawn object in the new scene
        _playerSpawn = GameObject.FindWithTag("PlayerSpawn");

        if (_playerSpawn != null)
        {
           
            // Respawn the player at the PlayerSpawn position
            RespawnPlayer(_playerSpawn.transform);
        }
        else
        {
          //  Debug.LogError("Player spawn point not found in the scene!");
        }
    }

    //void JournalReminder()
    //{
    //    if (remindAboutJournal)
    //    {
    //        messageText.text = "Press J to Review your Journal";
    //        messageText.gameObject.SetActive(true);
    //        StartCoroutine(WaitForPlayerToCloseMessage());

    //    }
       
    //}

    //private IEnumerator WaitForPlayerToCloseMessage()
    //{
    //    // Wait until the player presses the Escape key
    //    while (!Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        yield return null;
    //    }

    //    // Deactivate the message text when the player presses Escape
    //    messageText.gameObject.SetActive(false);
    //}
}
