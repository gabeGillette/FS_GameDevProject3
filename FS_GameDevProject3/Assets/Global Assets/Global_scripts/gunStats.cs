using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AmmoType
{
    Pistol,
    Shotgun,
    MachineGun,
    // Add more ammo types as needed
}


[CreateAssetMenu]



public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public int ammoCur, ammoMax, ammoRes;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootVol;
    public AmmoType ammoType;

    //  public GameObject bulletPrefab;
}
