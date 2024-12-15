using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform headPos; // For raycasting vision
    [SerializeField] private Transform meleePos; // Position to spawn melee hitbox
    [SerializeField] private GameObject meleeHit; // Melee attack object prefab

    [Header("Stats")]
    [SerializeField] private int HP;
    [SerializeField] private float meleeRate; // Time between attacks
    [SerializeField] private float roamDist; // Roaming range
    [SerializeField] private float viewAngle; // Field of view angle
    [SerializeField] private float faceTargetSpeed;

    [Header("Roaming Settings")]
    [SerializeField] private float roamTimer;

    private Vector3 startingPos; // Initial position for roaming
    private bool isAttacking = false; // Attack state
    private bool isRoaming = false; // Roaming state
    private bool playerInRange = false; // If player is within trigger
    private float stoppingDistOrig; // Store original stopping distance
    private Vector3 playerDir; // Direction to player
    private float angleToPlayer; // Angle to player

    public itemDropOnDeath itemDrop;


    private void Start()
    {
        GameObject itemDropObject = GameObject.FindGameObjectWithTag("ItemDropper");
        itemDrop = itemDropObject.GetComponent<itemDropOnDeath>();

        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    private void Update()
    {
        // Smoothly update animator speed parameter
        float agentSpeed = agent.velocity.magnitude;
        anim.SetFloat("Speed", agentSpeed);

        // Roam if no player in range or visible
        if (!playerInRange || !CanSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance <= agent.stoppingDistance)
            {
                StartCoroutine(Roam());
            }
        }
    }

    private IEnumerator Roam()
    {
        isRoaming = true;

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayBackgroundMusic();
        }

        yield return new WaitForSeconds(roamTimer);

        agent.stoppingDistance = 0;
        Vector3 randomPoint = Random.insideUnitSphere * roamDist + startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
    }

    private bool CanSeePlayer()
    {
        if (GameManager.Instance.Player == null || headPos == null)
        {
        //    Debug.LogWarning("Player reference is missing in GameManager!");
            return false;
        }

        playerDir = GameManager.Instance.Player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.stoppingDistance = stoppingDistOrig; // Set appropriate stopping distance
                // Player is visible, move towards them
                agent.SetDestination(GameManager.Instance.Player.transform.position);

                // Play chase music if not already playing
                if (MusicManager.Instance != null)
                {
                    Debug.Log("Enemy sees the player, switching to chase music");
                    MusicManager.Instance.PlayChaseMusic();
                }

                // Attack when close enough
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.isStopped = true;
                    FaceTarget();
                    if (!isAttacking)
                    {
                        StartCoroutine(Attack());
                    }
                }
                else
                {
                    agent.isStopped = false;
                }

                return true;
            }
        }

        // Reset stopping distance and play background music when player is not visible
        agent.stoppingDistance = 0;
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayBackgroundMusic();
        }

        agent.stoppingDistance = 0;
        return false;
    }

    private void FaceTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * faceTargetSpeed);
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        // Create the melee hit object
        createMeleeHit();

        // Wait for the attack cooldown
        yield return new WaitForSeconds(meleeRate);

        anim.ResetTrigger("Attack");
        isAttacking = false;
    }

    public void createMeleeHit()
    {
        Quaternion spawnRotation = Quaternion.LookRotation(transform.forward); // Face forward
        GameObject hit = Instantiate(meleeHit, meleePos.position, spawnRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            agent.stoppingDistance = stoppingDistOrig; // Restore stopping distance
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
            agent.isStopped = false;
        }
    }

    public void TakeDamage(int damage)
    {
       // Debug.Log("Enemy Health: " + HP);
        HP -= damage;
        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        anim.SetTrigger("Death");
        agent.isStopped = true;
        itemDrop.DropRandomItem(transform.position);
        Destroy(gameObject, 2f); // Delay to allow death animation to play
    }
}