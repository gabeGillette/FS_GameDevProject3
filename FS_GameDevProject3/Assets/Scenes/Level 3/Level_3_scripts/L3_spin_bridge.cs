using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L3_spin_bridge : MonoBehaviour
{
    [SerializeField] float _speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, _speed * Time.deltaTime, 0);
        
    }


}
