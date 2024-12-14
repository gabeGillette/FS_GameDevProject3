using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEditor.ProBuilder;

public class GameManager : MonoBehaviour
{
    /*------------------------------------------- SERIALIZED */

    [SerializeField] GameObject _playerPrefab;
    [SerializeField] Image _reticle;

    [SerializeField] TMP_Text _UITopLeft;
    [SerializeField] TMP_Text _UITopRight;
    [SerializeField] TMP_Text _UIMessages;
    [SerializeField] TMP_Text _UIQuest;

    [Header("-----Menus-----")]
    [SerializeField] GameObject _menuActive;
    [SerializeField] GameObject _menuPause;

    [SerializeField] [Range(0, 20)] float _messageDuration;

    [SerializeField] TextAsset _versionFile;


    /*------------------------------------------ PRIVATE MEMBERS */

    static private GameManager _instance;

    private GameObject _player;
    private playerController _playerScript;
    private List<GameObject> _evidenceList;
    public GameObject _playerSpawn;

    private int _evidenceTotal;
    private int _evidenceCollected;

    private int _currentLevel;

    private List<string> _messageList = new List<string>();

    private bool _isPaused;
    private bool isDisplayed;
    private float _timeScale;

    // Store player data between scenes
    private int savedHP;
    private int savedGunIndex;
    private int savedAmmoCur;
    private int savedAmmoRes;



    /*------------------------------------------ PUBLIC ACCESSORS */

    public static GameManager Instance => _instance;
    public GameObject Player => _player;
    public playerController PlayerScript => _playerScript;

    /*--------------------------------------------- PRIVATE METHODS */

    // Start is called before the first frame update
    void Awake()
    {
        _isPaused = false;
        _timeScale = 1.0f;

        _currentLevel = SceneManager.GetActiveScene().buildIndex;

        // find the playerspawner
        _playerSpawn = GameObject.FindWithTag("PlayerSpawn");

        if (_playerSpawn != null)
        {
            if (_player == null)
            {
                RespawnPlayer(_playerSpawn.transform);

                SetPlayerReference();
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
            DontDestroyOnLoad(gameObject); // Persist across scenes
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
    }


    // Add a method to store player data
    public void SavePlayerData(playerController player)
    {
        // Save health
        PlayerPrefs.SetInt("PlayerHealth", player.HP);

        // Save selected gun index
        PlayerPrefs.SetInt("PlayerGunIndex", player._selectedGun);

        // Save current ammo in selected gun
        PlayerPrefs.SetInt("PlayerAmmoCur", player.SelectedGun.ammoCur);

        // Save ammo reserves
        PlayerPrefs.SetInt("PlayerAmmoRes", player.SelectedGun.ammoRes);

        // Save Evidence Collected
        PlayerPrefs.SetInt("TotalEvidenceCollected", _evidenceCollected);

        // Save player position
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", player.transform.position.z);

        // Debug log for evidence collected
        Debug.Log(_evidenceCollected);

        // Save the data to disk
        PlayerPrefs.Save();


        Debug.Log("Player Health: " + PlayerPrefs.GetInt("PlayerHealth"));
        Debug.Log("Player Gun Index: " + PlayerPrefs.GetInt("PlayerGunIndex"));
        Debug.Log("Player Ammo Current: " + PlayerPrefs.GetInt("PlayerAmmoCur"));
        Debug.Log("Player Ammo Reserves: " + PlayerPrefs.GetInt("PlayerAmmoRes"));
        Debug.Log("Evidence Collected: " + PlayerPrefs.GetInt("TotalEvidenceCollected"));
        Debug.Log("Player Position: " + PlayerPrefs.GetFloat("PlayerPosX") + ", " +
                  PlayerPrefs.GetFloat("PlayerPosY") + ", " + PlayerPrefs.GetFloat("PlayerPosZ"));

    }

    // Add a method to load player data
    public void LoadPlayerData(playerController player)
    {
        // Load saved player data from PlayerPrefs
        int loadedHP = PlayerPrefs.GetInt("PlayerHealth", player.HP); // Default to current health if no saved value
        int loadedGunIndex = PlayerPrefs.GetInt("PlayerGunIndex", player._selectedGun); // Default to current gun index if no saved value
        int loadedAmmoCur = PlayerPrefs.GetInt("PlayerAmmoCur", player.SelectedGun.ammoCur); // Default to current ammo if no saved value
        int loadedAmmoRes = PlayerPrefs.GetInt("PlayerAmmoRes", player.SelectedGun.ammoRes); // Default to current ammo reserve if no saved value

        // Load player position
        float loadedPosX = PlayerPrefs.GetFloat("PlayerPosX", player.transform.position.x); // Default to current position if no saved value
        float loadedPosY = PlayerPrefs.GetFloat("PlayerPosY", player.transform.position.y);
        float loadedPosZ = PlayerPrefs.GetFloat("PlayerPosZ", player.transform.position.z);

        // Debug logs to check loaded data
        Debug.Log("Loading Player Data:");
        Debug.Log("Loaded Health: " + loadedHP);
        Debug.Log("Loaded Gun Index: " + loadedGunIndex);
        Debug.Log("Loaded Ammo Current: " + loadedAmmoCur);
        Debug.Log("Loaded Ammo Reserve: " + loadedAmmoRes);
        Debug.Log("Loaded Position: " + new Vector3(loadedPosX, loadedPosY, loadedPosZ));

        // Apply loaded data to the player
        player.SetHealth(loadedHP); // Apply the loaded health
        player.SetGun(loadedGunIndex); // Apply the loaded gun index
        player.SetAmmo(loadedAmmoCur, loadedAmmoRes); // Apply the loaded ammo counts

        // Set player position
        player.transform.position = new Vector3(loadedPosX, loadedPosY, loadedPosZ);
       // Scene currentScene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(_currentLevel);
        Debug.Log("Player data loaded successfully.");
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
        Debug.Log("Collected " + pickup.name);

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
        LoadPlayerData(_player.GetComponent<playerController>());

    }



    private void SetPlayerReference()
    {
        // Find the player
        _player = GameObject.FindWithTag("Player");

        if (_player == null)
        {
            Debug.LogError("Player does not exist!");
        }
        else
        {
            Debug.Log("Player found successfully!");
        }

        _player.TryGetComponent<playerController>(out _playerScript);
        if (_playerScript == null)
        {
            Debug.LogError("Player is missing PlayerController!");
        }
        else
        {
            Debug.Log("PlayerController script assigned successfully!");
        }
    }


    public void UpdateUI()
    {
        _UITopLeft.text = ($"Health: {PlayerScript.HP}\n" +
            $"Ammo: {PlayerScript.SelectedGun.ammoCur} / {PlayerScript.SelectedGun.ammoRes}\n" +
            $"Evidence: {_evidenceCollected}/{_evidenceTotal}\n" +
            $"Monsters Spawned: {0}/{0}\n" +
            $"Monsters Killed: {0}/{0}");

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
        _menuActive = _menuPause;
        _menuActive.SetActive(true);
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UnpauseGame()
    {
        _isPaused = false;
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
            Debug.LogError("Player spawn point not found in the scene!");
        }
    }
}
