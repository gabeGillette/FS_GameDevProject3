using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*------------------------------------------ PRIVATE MEMBERS */

    static private GameManager _instance;

    private GameObject _player;

    /*------------------------------------------ PUBLIC ACCESSORS */

    public static GameManager Instance => _instance;
    public GameObject Player => _player;

    /*--------------------------------------------- PRIVATE METHODS */

    // Start is called before the first frame update
    void Awake()
    {
        _player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
