using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamage
{

    [SerializeField] int baseHealth;

    public void takeDamage(int amount)
    {
        baseHealth -= amount;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (baseHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
