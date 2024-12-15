using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [SerializeField] int _dmg_amt;
    void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<IDamage>() != null)
        {
            other.GetComponent<IDamage>().TakeDamage(_dmg_amt);
        }
    }
}
