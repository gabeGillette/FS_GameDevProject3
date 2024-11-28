using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemeyWandererAI : MonoBehaviour
{
    // State Machine
    private enum State { Roaming, Investigating, Chasing, Attacking, Searching }
    private State currentState;

    // References
    public Transform player;
    public LayerMask playerLayer;
    public LayerMask hidingSpotsLayer;
    public AudioSource hearingRange;

    // Patrol Variables
    public float roamRadius = 10f;
    public float roamSpeed = 2f;
    private Vector3 roamTarget;

    // Chase Variables
    public float chaseSpeed = 5f;
    public float chaseRange = 10f;

    // Noise Variables
    public float investigateRange = 15f;
    public float gunshotRange = 30f;

    // Hiding Mechanic
    private bool playerHiding = false;
    private Transform hidingSpot;

    void Start()
    {
        currentState = State.Roaming;
        PickNewRoamTarget();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Roaming:
                HandleRoaming();
                break;
            case State.Investigating:
                HandleInvestigating();
                break;
            case State.Chasing:
                HandleChasing();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
            case State.Searching:
                HandleSearching();
                break;
        }
    }

    // Roaming Logic
    void HandleRoaming()
    {
        MoveTo(roamTarget, roamSpeed);

        if (Vector3.Distance(transform.position, roamTarget) < 1f)
            PickNewRoamTarget();

        if (PlayerMakesNoise(out float noiseRange))
        {
            if (noiseRange <= investigateRange)
                ChangeState(State.Investigating);
        }

        if (CanSeePlayer())
            ChangeState(State.Chasing);
    }

    void PickNewRoamTarget()
    {
        roamTarget = new Vector3(
            transform.position.x + Random.Range(-roamRadius, roamRadius),
            transform.position.y,
            transform.position.z + Random.Range(-roamRadius, roamRadius)
        );
    }

    // Investigating Logic
    void HandleInvestigating()
    {
        MoveTo(player.position, roamSpeed);

        if (Vector3.Distance(transform.position, player.position) < chaseRange)
            ChangeState(State.Chasing);

        // Return to roaming if nothing is found after time
        // Add timer logic here
    }

    // Chasing Logic
    void HandleChasing()
    {
        MoveTo(player.position, chaseSpeed);

        if (PlayerIsHiding(out hidingSpot))
            ChangeState(State.Searching);

        if (Vector3.Distance(transform.position, player.position) < 1.5f)
            ChangeState(State.Attacking);
    }

    // Attacking Logic
    void HandleAttacking()
    {
        // Damage logic here

        // Return to roaming if player escapes
        if (!CanSeePlayer())
            ChangeState(State.Roaming);
    }

    // Searching Logic
    void HandleSearching()
    {
        if (hidingSpot != null && CanSeePlayerEnterHidingSpot())
        {
            PullPlayerOut();
        }
        else
        {
            // Search around hiding spot for a while
            // Add search behavior
            ChangeState(State.Roaming);
        }
    }

    // Helper Methods
    bool PlayerMakesNoise(out float noiseRange)
    {
        // Detect player noise level
        noiseRange = Vector3.Distance(transform.position, player.position); // Placeholder
        return noiseRange < gunshotRange; // Adjust logic here
    }

    bool CanSeePlayer()
    {
        // Define a LayerMask that excludes the HidingSpot layer
        int layerMask = ~(1 << LayerMask.NameToLayer("HidingSpot")); // Excludes HidingSpot

        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, chaseRange, layerMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true; // Player is visible
            }
        }
        return false; // Player is not visible
    }

    bool PlayerIsHiding(out Transform hidingSpot)
    {
        Collider[] spots = Physics.OverlapSphere(player.position, 1f, hidingSpotsLayer);
        hidingSpot = spots.Length > 0 ? spots[0].transform : null;
        return hidingSpot != null;
    }

    bool CanSeePlayerEnterHidingSpot()
    {
        // Logic for seeing player enter hiding spot
        return true; // Placeholder
    }

    void PullPlayerOut()
    {
        // Pull the player out of hiding
    }

    void MoveTo(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    void ChangeState(State newState)
    {
        currentState = newState;
    }
}