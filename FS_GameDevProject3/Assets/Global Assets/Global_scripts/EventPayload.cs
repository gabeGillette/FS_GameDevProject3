using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPayload : MonoBehaviour
{

    [SerializeField] UnityEvent _payload;


    public UnityEvent Payload => _payload;
}
