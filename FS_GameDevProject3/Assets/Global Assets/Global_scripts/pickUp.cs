using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour
{
    enum pickupType { gun, HP, stamina, grenade }
    [SerializeField] pickupType type;
    [SerializeField] gunStats gun;

    // Start is called before the first frame update
    void Start()
    {
        if (type == pickupType.gun)
        {
            gun.ammoCur = gun.ammoMax;
        }
    }

    ///Uncomment when GameManager has a reference added for the PlayerScript///
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        GameManager.instance.playerScript.getGunStats(gun);
    //        Destroy(gameObject);
    //    }
    //}

}
