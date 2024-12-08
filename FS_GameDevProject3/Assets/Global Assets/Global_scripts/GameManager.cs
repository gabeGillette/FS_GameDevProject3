using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /*------------------------------------------- SERIALIZED */

    [SerializeField] GameObject _playerPrefab;
    [SerializeField] Image _reticle;

    [SerializeField] TMP_Text _UITopLeft;
    [SerializeField] TMP_Text _UITopRight;
    [SerializeField] TMP_Text _UIMessages;

    [SerializeField] [Range(0, 20)] float _messageDuration;

    [SerializeField] TextAsset _versionFile;


    /*------------------------------------------ PRIVATE MEMBERS */

    static private GameManager _instance;

    private GameObject _player;
    private playerController _playerScript;
    private List<GameObject> _evidenceList;
    private GameObject _playerSpawn;

    private int _evidenceTotal;
    private int _evidenceCollected;

    private int _currentLevel;

    private List<string> _messageList = new List<string>();

    private bool _isPaused;
    private float _timeScale;

    

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
            RespawnPlayer(_playerSpawn.transform);
        }

        // find all the evidence interactables
        _evidenceList = new List<GameObject>();
        _evidenceList.AddRange(GameObject.FindGameObjectsWithTag("Evidence"));

        _evidenceCollected = 0;
        _evidenceTotal = _evidenceList.Count;

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject); // Avoid multiple instances
        }

        UpdateUI();
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isPaused) {
                PauseGame();
            }
            else
            {
                UnpauseGame();
            }
                   
        }
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

        foreach(string message in _messageList)
        {
            fullMessage += message + "\n";
        }

        _UIMessages.text = fullMessage;
    }

    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UnpauseGame()
    {
        _isPaused = false;
        Time.timeScale = _timeScale;
        Cursor.lockState = CursorLockMode.Locked;
    }



}
