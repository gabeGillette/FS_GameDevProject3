using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSaveChecker : MonoBehaviour
{
    // Start is called before the first frame update

    public bool _isLoad;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
