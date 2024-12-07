using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class pickUp : MonoBehaviour, IPickup
{
    enum pickupType { gun, HP, stamina, battery, evidence, pistolammo, shotgunammo, tommygunammo, key }
    [SerializeField] pickupType type;
    [SerializeField] gunStats gun;
    [SerializeField] [Range(5,50)] int healthPackAmount;
    [SerializeField] int ammoPickup;
    [SerializeField] int batteryRecharge;

    public AudioClip evidenceSound;
    public AudioClip ammoPickUpSound;


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
        GameObject flashLight = GameObject.FindGameObjectWithTag("FlashLight");

        GameObject LevelController = GameObject.FindGameObjectWithTag("LevelController");
        if (player != null)
        {
            playerController playerControllerScript = player.GetComponent<playerController>();
            GameManager gameManager = FindObjectOfType<GameManager>();

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
                playerControllerScript.getGunStats(gun);
                Destroy(gameObject);

            }
            else if (type == pickupType.evidence)
            {
                AudioSource.PlayClipAtPoint(evidenceSound, transform.position);
                Destroy(gameObject);


                gameManager.CollectEvidence();

            }
            else if(type == pickupType.pistolammo)
            {
                AudioSource.PlayClipAtPoint(ammoPickUpSound, transform.position);

                playerControllerScript.returnAmmo(ammoPickup, AmmoType.Pistol);
                Destroy(gameObject);
            }
            else if (type == pickupType.shotgunammo)
            {
                AudioSource.PlayClipAtPoint(ammoPickUpSound, transform.position);

                playerControllerScript.returnAmmo(ammoPickup, AmmoType.Shotgun);
                Destroy(gameObject);
            }
            else if(type == pickupType.key)
            {
                LevelRequirement levelRequirements = LevelController.GetComponent<LevelRequirement>();
                levelRequirements.hasKey = true;
                Destroy(gameObject);

            }
            else if (type == pickupType.battery)
            {
                FlashlightToggle batteryLevel = flashLight.GetComponent<FlashlightToggle>();
                batteryLevel.batteryLife += batteryRecharge;
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
