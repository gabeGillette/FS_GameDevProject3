using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikeDamage : MonoBehaviour
{
    public int damageAmount = 10; // Amount of damage dealt to the player


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the colliding object is the player
        {
            playerController playerControllerScript = other.GetComponent<playerController>();

            if (playerControllerScript != null)
            {
                playerControllerScript.takeDamage(damageAmount);
                Debug.Log("Player took " + damageAmount + " damage");
            }
            else
            {
                Debug.LogWarning("playerController script not found on player.");
            }
        }
    }
}
