using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDeath : MonoBehaviour
{
   

    private void OnTriggerEnter(Collider other)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerController playerControllerScript = player.GetComponent<playerController>();

        playerControllerScript.Death();
    }
}
