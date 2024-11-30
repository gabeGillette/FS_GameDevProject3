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

        if (playerInRange && !canSeePlayer())
        {
            Debug.Log("Player is near, but not visible. Enemy roaming.");
            if (!isRoaming && agent.remainingDistance < 0.05f)
                StartCoroutine(roam());
        }
        else if (!playerInRange)
        {
            Debug.Log("Player is out of range. Enemy roaming.");
            if (!isRoaming && agent.remainingDistance < 0.05f)
                StartCoroutine(roam());
        }
    }

    private void HandleAnimationSpeed()
    {
        float agentSpeed = agent.velocity.magnitude; // Use actual magnitude
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animSpeedTrans));
    }

    private IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);

        ResetStoppingDistance();

        Vector3 randomDist = Random.insideUnitSphere * roamDist;
        randomDist += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDist, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
    }

    private bool canSeePlayer()
    {
        if (GameManager.Instance.Player == null) return false; // Handle case where player is null

        playerDir = GameManager.Instance.Player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir * 50f, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit, Mathf.Infinity, detectionLayer))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.Instance.Player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();

                    if (!isAttacking)
                    {
                        StartCoroutine(meleeAttack());
                    }
                }

                return true;
            }
        }
        return false;
    }

    private void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ResetStoppingDistance();
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
            if (player.CompareTag("Player"))
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
        // Visualize melee attack range in the Scene view
        if (AttackPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(AttackPos.position, agent.stoppingDistance);
        }
    }
}