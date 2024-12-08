using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FlashlightToggle : MonoBehaviour
{
    public GameObject lightGO; //light gameObject to work with
    private bool isOn = false; //is flashlight on or off?
    public float batteryUsageRate = 0.1f; // How much battery is used per second
    public float batteryLife; // Maximum battery life
    public float batteryLifeMax = 100f;

    // Use this for initialization
    void Start()
    {
        // Set default off
        lightGO.SetActive(isOn);
        batteryLife = batteryLifeMax;
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("Battery Life: " + batteryLife);

        if (batteryLife > batteryLifeMax)
        {
            batteryLife = batteryLifeMax;
        }
        // Toggle flashlight on key press (F key)
        if (Input.GetKeyDown(KeyCode.F) && batteryLife > 0)
        {
            // Toggle light state
            isOn = !isOn;

            if (isOn)
            {

                lightGO.SetActive(true); // Turn the light on
            }
            else
            {
                lightGO.SetActive(false); // Turn the light off
            }
        }

        // If flashlight is on, consume battery over time
        if (isOn && batteryLife > 0)
        {
            batteryLife -= batteryUsageRate * Time.deltaTime; // Decrease battery life over time
            if (batteryLife <= 0)
            {
                batteryLife = 0; // Ensure battery doesn't go negative
                isOn = false; // Automatically turn off flashlight when battery is empty
                lightGO.SetActive(false); // Turn off light if battery is empty
            }
        }
    }
}
