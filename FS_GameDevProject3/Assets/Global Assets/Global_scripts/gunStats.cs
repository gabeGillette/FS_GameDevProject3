using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject bulletPrefab;
}
