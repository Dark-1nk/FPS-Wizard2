using UnityEngine;
using UnityEngine.AI;

public class SkeletonAI : MonoBehaviour
{
    private Transform player;
    public float detectionRange = 10f;
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float searchDuration = 5f;
    public float attackCooldown = 2f;
    public bool isChasing;
    bool soundPlayed = false;

    public AudioClips sfx;
    private NavMeshAgent agent;
    private Vector3 spawnPoint;
    private float lastSeenTime;
    private float lastAttackTime;

    private enum State { Patrolling, Chasing, Searching, Attacking };
    private State currentState = State.Patrolling;

    public Transform[] patrolPoints;
    private int currentPatrolIndex;
    PlayerMove targetPlayer;

    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = FindObjectOfType<PlayerMove>().transform;
        targetPlayer = FindObjectOfType<PlayerMove>();
        agent = GetComponent<NavMeshAgent>();
        spawnPoint = transform.position;
        currentPatrolIndex = 0;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        animator.SetBool("isChasing", isChasing);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInSight = IsPlayerInSight();

        switch (currentState)
        {
            case State.Patrolling:
                if (playerInSight && distanceToPlayer <= detectionRange)
                {
                    currentState = State.Chasing;
                }
                else if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    GoToNextPatrolPoint();
                }
                break;

            case State.Chasing:
                if (playerInSight || distanceToPlayer < chaseRange)
                {
                    if (!soundPlayed)
                    {
                        sfx.PlayOneShot("Skeleton");
                        soundPlayed = true;
                    }

                    isChasing = true;
                    lastSeenTime = Time.time;
                    agent.SetDestination(player.position);

                    if (distanceToPlayer <= attackRange)
                    {
                        sfx.StopClip(1f);
                        soundPlayed = false;
                        currentState = State.Attacking;
                    }
                }

                if (Time.time - lastSeenTime > searchDuration)
                    {
                        sfx.StopClip(2f);
                        soundPlayed = false;
                        currentState = State.Searching;
                    }
                break;

            case State.Searching:
                if (Time.time - lastSeenTime > searchDuration)
                {
                    agent.SetDestination(spawnPoint);
                    if (Vector3.Distance(transform.position, spawnPoint) < 1f)
                    {
                        currentState = State.Patrolling;
                        GoToNextPatrolPoint();
                    }
                }
                else
                {
                    agent.SetDestination(player.position);
                }
                break;

            case State.Attacking:
                if (distanceToPlayer <= attackRange)
                {
                    agent.isStopped = true;
                    AttackPlayer(targetPlayer);
                }
                else if (distanceToPlayer > attackRange)
                {
                    agent.isStopped = false;
                    currentState = State.Chasing;
                }
                break;
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private bool IsPlayerInSight()
    {
        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out RaycastHit hit, detectionRange))
        {
            return hit.collider.GetComponent<PlayerMove>();
        }
        return false;
    }

    private void AttackPlayer(PlayerMove playerMove)
    {
        if (player.GetComponent<PlayerMove>() != null && Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            playerMove.TakeDamage(1);
            lastAttackTime = Time.time;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
