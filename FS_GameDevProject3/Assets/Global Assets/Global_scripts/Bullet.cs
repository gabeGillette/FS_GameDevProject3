using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 30f; // Adjust this to control bullet speed
    [SerializeField] float lifeTime = 5f; // Time (in seconds) before the bullet is destroyed if it doesn't hit anything

    // Reference to the bullet's Rigidbody (if needed for physics-based movement)
    private Rigidbody _rigidbody;
    private float _timeAlive; // Timer to keep track of how long the bullet has been alive

    // Optionally, you can use this to deal damage or create effects
    public int damage = 10; // Example damage, adjust based on your needs

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>(); // If needed for physics
        _timeAlive = 0f; // Initialize the time timer to 0

    }

    private void Update()
    {
        _timeAlive += Time.deltaTime;

        // If the bullet has been alive for longer than its lifetime, destroy it
        if (_timeAlive >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    

    // This method is triggered when the bullet hits something
    void OnCollisionEnter(Collision collision)
    {
        // Check if the bullet hit something
        // You can check for specific tags or types of objects if needed
        Debug.Log("Bullet hit: " + collision.collider.name);

        // Apply damage to the object if it implements the IDamage interface
        IDamage dmg = collision.collider.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damage);
        }

        // Destroy the bullet after the collision
        Destroy(gameObject);
    }

    // Alternatively, use OnTriggerEnter if the collider is set as a trigger
    void OnTriggerEnter(Collider other)
    {
        // Check if the bullet hit something
        Debug.Log("Bullet hit: " + other.name);

        // Apply damage or any effects as needed
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damage);
        }

        // Destroy the bullet after the trigger event
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        // If you are using a Rigidbody, apply force to make the bullet move
        if (_rigidbody != null)
        {
            // Move the bullet forward using its own forward direction and speed
            _rigidbody.velocity = transform.forward * bulletSpeed; // Adjusts velocity directly
        }
    }
}
