using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Renderer model;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform AttackPos;
    [SerializeField] private Transform headPos;

    [SerializeField] private int HP;
    [SerializeField] private int faceTargetSpeed;
    [SerializeField] private int viewAngle;
    [SerializeField] private int roamDist;
    [SerializeField] private int roamTimer;
    [SerializeField] private int animSpeedTrans;

    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private string playerTag = "Player"; // Set this in Inspector


    [SerializeField] private float attackRate;

    private bool isAttacking;
    private bool playerInRange;
    private bool isRoaming;

    private Vector3 playerDir;
    private Vector3 startingPos;

    private float angleToPlayer;
    private float stoppingDistOrig;

    // Start is called before the first frame update
    void Start()
    {
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnimationSpeed();

        if (canSeePlayer())
        {
            Debug.Log("Enemy is chasing the player."); // Logs when chase logic is triggered
            agent.SetDestination(GameManager.Instance.Player.transform.position);

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                anim.SetBool("isWalking", true); // Trigger walking animation
            }
            else
            {
                anim.SetBool("isWalking", false); // Stop walking animation
                if (!isAttacking)
                {
                    StartCoroutine(meleeAttack());
                }
            }
        }
        else if (!isRoaming)
        {
            Debug.Log("Enemy is roaming."); // Logs when roaming logic starts
            StartCoroutine(roam());
        }
    }



    private void HandleAnimationSpeed()
    {
        float agentSpeed = agent.velocity.magnitude; // Use the actual velocity magnitude of the NavMeshAgent
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animSpeedTrans));
        Debug.Log($"Animator Speed: {anim.GetFloat("Speed")}"); // Log the Animator's Speed parameter
    }


    private IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);

        ResetStoppingDistance();

        // Select a random position within roam distance
        Vector3 randomDist = Random.insideUnitSphere * roamDist + startingPos;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDist, out hit, roamDist, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        isRoaming = false;
    }


    private void ChasePlayer()
    {
        Debug.Log("Player detected. Chasing...");
        agent.SetDestination(GameManager.Instance.Player.transform.position);

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            anim.SetBool("isWalking", true); // Trigger walking animation
        }
        else
        {
            anim.SetBool("isWalking", false); // Stop walking animation
            if (!isAttacking)
            {
                StartCoroutine(meleeAttack()); // Start melee attack
            }
        }
    }


    private bool canSeePlayer()
    {
        if (GameManager.Instance.Player == null) return false; // Ensure the player is assigned

        // Calculate direction to the player
        playerDir = GameManager.Instance.Player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // Log the angle to the player for debugging
        Debug.Log($"Angle to player: {angleToPlayer}");

        // Check if the player is within the view angle
        if (angleToPlayer <= viewAngle / 2f)
        {
            Debug.Log("Player is within view angle.");

            // Perform a raycast to check for obstacles
            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir, out hit, Mathf.Infinity, detectionLayer))
            {
                Debug.Log($"Raycast hit: {hit.collider.name}");

                // Check if the raycast hit the player
                if (hit.collider.CompareTag(playerTag))
                {
                    Debug.Log("Player detected. Enemy can see the player.");
                    return true; // Player is visible
                }
            }
        }

        // Player not detected or outside view angle
        Debug.Log("Player not detected or outside view angle.");
        return false;
    }





    private void AlertMusicManager(bool playerDetected)
    {
        if (playerDetected)
        {
            Debug.Log("Switching to chase music...");
            MusicManager.Instance.SwitchToChaseMusic();
        }
        else
        {
            Debug.Log("Switching to background music...");
            MusicManager.Instance.SwitchToBackgroundMusic();
        }
    }


    private void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            ResetStoppingDistance();
            agent.ResetPath();
        }
    }

    private IEnumerator meleeAttack()
    {
        isAttacking = true; // Prevent multiple melee attacks
        anim.SetTrigger("Attack");

        // Apply damage if the player is within range
        Collider[] hitPlayers = Physics.OverlapSphere(AttackPos.position, agent.stoppingDistance);
        foreach (Collider player in hitPlayers)
        {
            if (player.CompareTag(playerTag))
            {
                Debug.Log("Enemy attacks the player!");
                player.GetComponent<playerController>()?.takeDamage(10); // Adjust damage value
            }
        }

        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP > 0) // Only stagger if still alive
        {
            anim.SetTrigger("Hit"); // Play hit animation
        }

        if (HP <= 0)
        {
            anim.SetTrigger("Die"); // Play death animation
            Destroy(gameObject, 2f); // Delay destruction for animation
        }
    }

    private void ResetStoppingDistance()
    {
        agent.stoppingDistance = 0;
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (headPos != null)
        {
            // Visualize the view angle
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(headPos.position, 10f); // Adjust for detection range

            Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(headPos.position, leftBoundary * 5f); // Adjust for range
            Gizmos.DrawRay(headPos.position, rightBoundary * 5f); // Adjust for range
        }
    }
}