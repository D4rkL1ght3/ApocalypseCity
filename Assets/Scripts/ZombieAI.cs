using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float losePlayerRange = 18f;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolWaitTime = 2f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;

    private NavMeshAgent agent;
    private Vector3 spawnPosition;
    private float patrolTimer;
    private float attackTimer;
    private bool isChasingPlayer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spawnPosition = transform.position;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogWarning("ZombieAI could not find an object with the Player tag.");
            }
        }

        agent.speed = patrolSpeed;
        PickNewPatrolPoint();
    }

    private void Update()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= detectionRange)
        {
            isChasingPlayer = true;
        }
        else if (distanceToPlayer >= losePlayerRange)
        {
            isChasingPlayer = false;
        }

        if (isChasingPlayer)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                PickNewPatrolPoint();
                patrolTimer = 0f;
            }
        }
    }

    private void PickNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += spawnPosition;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        agent.speed = chaseSpeed;

        if (distanceToPlayer > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            FacePlayer();
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (attackTimer > 0f)
            return;

        IDamageable damageable = player.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(attackDamage);
        }

        attackTimer = attackCooldown;
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? spawnPosition : transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, patrolRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}