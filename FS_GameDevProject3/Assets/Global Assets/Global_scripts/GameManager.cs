using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*------------------------------------------ PRIVATE MEMBERS */

    static private GameManager _instance;

    private GameObject _player;

    private List<GameObject> _evidenceList;

    

    /*------------------------------------------ PUBLIC ACCESSORS */

    public static GameManager Instance => _instance;
    public GameObject Player => _player;

    /*--------------------------------------------- PRIVATE METHODS */

    // Start is called before the first frame update
    void Awake()
    {
        // find the player
        _player = GameObject.FindWithTag("Player");

        // find all the evidence interactables
        _evidenceList = new List<GameObject>();
        _evidenceList.AddRange(GameObject.FindGameObjectsWithTag("Evidence"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
