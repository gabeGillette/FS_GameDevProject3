using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class L3_move_bridge : MonoBehaviour
{
    [SerializeField] float _speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.Translate(new Vector3(_speed * Time.deltaTime, 0, 0));
        //gameObject.GetComponent<CharacterController>().Move(new Vector3 (_speed * Time.deltaTime, 0, 0));
        gameObject.GetComponent<CharacterController>().Move(new Vector3(_speed, 0, 0));
    }
}
