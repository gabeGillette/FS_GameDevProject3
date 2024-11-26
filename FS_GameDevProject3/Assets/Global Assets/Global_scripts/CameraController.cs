/// Authors: Gabriel Gillette
/// Desc: Camera Controller component

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*---------------------------------------------- SERIALIZED */

    [Header("Player Customizable Options")]

    [Tooltip("X-Axis Sensitivity")]
    [SerializeField][Range(0.0f, 500.0f)] float _xSensitivity;

    [Tooltip("Y-Axis Sensitivity")]
    [SerializeField][Range(0.0f, 500.0f)] float _ySensitivity;

    [Tooltip("Invert Aiming Direction")]
    [SerializeField] bool _invert;

    [Header("Game Designer Options")]

    [Tooltip("Max Y-Look in Degrees")]
    [SerializeField][Range(-180.0f, 180.0f)] float _maxYLook;

    [Tooltip("Max Y-Look in Degrees")]
    [SerializeField][Range(-180.0f, 180.0f)] float _minYLook;

    /*--------------------------------------- PRIVATE MEMBERS */

    // X axis rotation in degrees
    private float _rotX;

    /*---------------------------------------- PRIVATE METHODS */

    /// <summary>
    /// Component start event.
    /// </summary>
    void Start()
    {
        /// TODO: GameManager should handle camera lock state
        Cursor.lockState = CursorLockMode.Locked; 

        // Init value to 0
        _rotX = 0.0f;

        // log an error if the camera doesn't have a parent
        if(transform.parent == null)
        {
            Debug.LogError("CameraController component must have a parent!");
        }
    }


    /// <summary>
    /// Component update event.
    /// </summary>
    void Update()
    {
        Look(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }


    /// <summary>
    /// Look
    /// Move the camera around relative to input velocity.
    /// </summary>
    /// <param name="xVel">x-velocity</param>
    /// <param name="yVel">y-velocity</param>
    private void Look(float xVel, float yVel)
    {
        // Calculate movment deltas
        float movex = xVel * _xSensitivity * Time.deltaTime;
        float movey = yVel * _ySensitivity * Time.deltaTime;

        // Determine x-axis rotation
        _rotX += _invert ? movey : -movey;
        _rotX = Mathf.Clamp(_rotX, _minYLook, _maxYLook);

        // transform camera
        transform.localRotation = Quaternion.Euler(_rotX, 0, 0);

        if(transform.parent != null) // basic null check for parent
            transform.parent.Rotate(Vector3.up * movex);  
    }
}
