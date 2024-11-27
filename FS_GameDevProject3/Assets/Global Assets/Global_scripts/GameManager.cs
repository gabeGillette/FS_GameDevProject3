using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*------------------------------------------ PRIVATE MEMBERS */

    static private GameManager _instance;

    private GameObject _player;
    private PlayerController _playerScript;

    private List<GameObject> _evidenceList;

    private int _evidenceTotal;
    private int _evidenceCollected;

    

    /*------------------------------------------ PUBLIC ACCESSORS */

    public static GameManager Instance => _instance;
    public GameObject Player => _player;
    public PlayerController PlayerScript => _playerScript;

    /*--------------------------------------------- PRIVATE METHODS */

    // Start is called before the first frame update
    void Awake()
    {
        // find the player
        _player = GameObject.FindWithTag("Player");
        _playerScript = _player.GetComponent<PlayerController>();

        if (_playerScript == null)
        {
            Debug.LogError("Player is missing PlayerController!");
        }

        // find all the evidence interactables
        _evidenceList = new List<GameObject>();
        _evidenceList.AddRange(GameObject.FindGameObjectsWithTag("Evidence"));

        _evidenceCollected = 0;
        _evidenceTotal = _evidenceList.Count;
    }

    
    public void CollectEvidence()
    {
        _evidenceCollected++;
    }

    public void GetPickup(GameObject pickup)
    {
        // TODO display ui message

        // just logging for now
        Debug.Log("Collected " + pickup.name);

        // get component
        IPickup pickupScript = pickup.GetComponent<IPickup>();

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


}
