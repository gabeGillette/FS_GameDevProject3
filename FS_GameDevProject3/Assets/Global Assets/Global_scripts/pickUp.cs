using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour, IPickup
{
    enum pickupType { gun, HP, stamina, grenade }
    [SerializeField] pickupType type;
    [SerializeField] gunStats gun;
    [SerializeField] [Range(5,50)] int healthPackAmount;

   

    // Start is called before the first frame update
    void Start()
    {
        if (type == pickupType.gun)
        {
            gun.ammoCur = gun.ammoMax;
        }
    }

    public void CollectEvent()
    {
        // Find the player (assuming the player has a PlayerHealth script)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController playerControllerScript = player.GetComponent<playerController>();
            if (type == pickupType.HP)
            {
                Debug.Log("Player Cur Health" + playerControllerScript.HP + "  Player Max Health:" + playerControllerScript.HPOriginal);
                
                if (playerControllerScript.HP < playerControllerScript.HPMax)
                {
                    playerControllerScript.restoreHealth(healthPackAmount);  // Restore health to the player
                    Destroy(gameObject);

                }
            }
            else if (type == pickupType.gun)
            {
                //   playerControllerScript.PickUpGun(gun);
                Destroy(gameObject);

            }
        }

        // Optionally destroy the pickup object after it's collected
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectEvent();  // Trigger the collection event when the player collides with the pickup
        }
    }

}
