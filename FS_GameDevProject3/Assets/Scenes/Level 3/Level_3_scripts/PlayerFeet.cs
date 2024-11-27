using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    private bool _isColliding;
    private Collider _collider;

    public bool IsColliding { get { return _isColliding; } }

    public Collider What { get { return _collider; } }

    private void OnTriggerEnter(Collider other)
    {
        _isColliding = true;
        _collider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        _isColliding = false;
        _collider = other;
    }
}
