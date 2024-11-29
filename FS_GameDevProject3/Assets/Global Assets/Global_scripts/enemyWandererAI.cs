using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyWandererAI : MonoBehaviour
{
    // References
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim; // Animator for animations
    [SerializeField] private Transform headPos;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask hidingSpotLayer;
    [SerializeField] private LayerMask noiseDetectionLayer;

    // Stats
    [SerializeField] private int HP = 100;
    [SerializeField] private float roamSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float roamDistance = 10f;
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float meleeRate = 2f; // Time between melee attacks
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private float investigateTime = 5f;
    [SerializeField] private float hearingRange = 15f;

    // Internal state
    private Vector3 startPosition;
    private bool isRoaming = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isInvestigating = false;
    private float nextAttackTime = 0f;

    void Start()
    {
        startPosition = transform.position;

        // Ensure the agent starts with roaming speed
        agent.speed = roamSpeed;
    }

    void Update()
    {
        if (!isChasing && !isInvestigating)
        {
            if (!isRoaming)
                StartCoroutine(Roam());
        }

        if (CanSeePlayer())
        {
            ChasePlayer();
        }

        // Update walking animation
        UpdateWalkingAnimation();
    }

    private IEnumerator Roam()
    {
        isRoaming = true;

        // Wait a moment before picking a new destination
        yield return new WaitForSeconds(Random.Range(1, 3));

        Vector3 randomPoint = Random.insideUnitSphere * roamDistance + startPosition;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, roamDistance, NavMesh.AllAreas))
        {
            agent.speed = roamSpeed;
            agent.SetDestination(hit.position);
        }

        isRoaming = false;
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - headPos.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < viewAngle / 2f)
        {
            RaycastHit hit;
            if (Physics.Raycast(headPos.position, directionToPlayer, out hit, Mathf.Infinity, playerLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    // Check if the player is hiding
                    Collider[] hidingSpots = Physics.OverlapSphere(player.position, 0.5f, hidingSpotLayer);
                    if (hidingSpots.Length == 0) // No hiding spots nearby
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void ChasePlayer()
    {
        isChasing = true;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        if (agent.remainingDistance <= meleeRange)
        {
            FacePlayer();
            if (Time.time >= nextAttackTime)
            {
                MeleeAttack();
                nextAttackTime = Time.time + meleeRate;
            }
        }
        else
        {
            isAttacking = false;
        }
    }

    private void MeleeAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack"); // Play attack animation
        Debug.Log("Enemy attacks the player!");
        // Add damage application to the player here
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private IEnumerator Investigate(Vector3 soundPosition)
    {
        isInvestigating = true;
        agent.SetDestination(soundPosition);

        yield return new WaitForSeconds(investigateTime);

        isInvestigating = false;
    }

    public void HearNoise(Vector3 noisePosition)
    {
        if (!isChasing)
        {
            StartCoroutine(Investigate(noisePosition));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            agent.speed = roamSpeed; // Return to roaming speed
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        anim.SetTrigger("Hit"); // Play hit animation

        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy defeated!");
        anim.SetTrigger("Die"); // Play death animation
        Destroy(gameObject, 2f); // Delay destruction to let the death animation play
    }

    private void UpdateWalkingAnimation()
    {
        float speedPercent = agent.velocity.magnitude / chaseSpeed;
        anim.SetFloat("Speed", speedPercent); // Assuming the Animator has a "Speed" parameter
    }
}
