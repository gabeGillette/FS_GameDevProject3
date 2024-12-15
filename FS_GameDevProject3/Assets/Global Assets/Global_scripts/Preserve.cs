using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preserve : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Keep this object across scenes
    }
}
