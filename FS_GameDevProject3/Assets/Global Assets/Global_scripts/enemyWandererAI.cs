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
    [SerializeField] private Transform attackPos;
    [SerializeField] private Transform headPos;

    [Header("-- Enemy Settings --")]
    [SerializeField] private int HP = 100;
    [SerializeField] private float faceTargetSpeed = 5f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private float roamDist = 10f;
    [SerializeField] private float roamTimer = 3f;
    [SerializeField] private float animSpeedTrans = 2f;

    [Header("-- Attack Settings --")]
    [SerializeField] private float stoppingDistance = 2f; // Adjustable attack range
    [SerializeField] private int damage = 10; // Adjustable damage output
    [SerializeField] private float attackRate = 3f;

    [Header("-- Detection Settings --")]
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private string playerTag = "Player"; // Ensure this matches the player's tag

    private bool isAttacking;
    private bool playerInRange;
    private bool isRoaming;

    private Vector3 playerDir;
    private Vector3 startingPos;

    private float angleToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        agent.stoppingDistance = stoppingDistance; // Apply stopping distance
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnimationSpeed();

        if (playerInRange && canSeePlayer())
        {
            Debug.Log("Enemy is chasing the player.");
            agent.SetDestination(GameManager.Instance.Player.transform.position);

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isWalking", false);
                if (!isAttacking)
                {
                    StartCoroutine(MeleeAttack());
                }
            }
        }
        else if (!playerInRange || !canSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
            {
                Debug.Log("Enemy roaming...");
                StartCoroutine(roam());
            }
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
                StartCoroutine(MeleeAttack()); // Start melee attack
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

    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        Collider[] hitPlayers = Physics.OverlapSphere(attackPos.position, stoppingDistance);
        foreach (Collider player in hitPlayers)
        {
            if (player.CompareTag(playerTag))
            {
                Debug.Log("Enemy is attacking the player!");
                player.GetComponent<playerController>()?.takeDamage(damage);
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