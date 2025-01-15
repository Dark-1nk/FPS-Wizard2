using UnityEngine;
using UnityEngine.AI;

public class GoblinAI : MonoBehaviour
{
    private Transform player;
    public float detectionRange = 10f;
    public float chaseRange = 15f;
    public float bombThrowRange = 8f;
    public float searchDuration = 5f;
    public float throwCooldown = 3f;
    public GameObject bombPrefab;
    public Transform bombSpawnPoint;
    public float bombExplosionRadius = 5f;
    public int bombDamage = 2;

    private NavMeshAgent agent;
    private Vector3 spawnPoint;
    private float lastSeenTime;
    private float lastThrowTime;

    private enum State { Patrolling, Chasing, Searching, ThrowingBomb };
    private State currentState = State.Patrolling;

    public Transform[] patrolPoints;
    private int currentPatrolIndex;

    private Animator animator;
    public AudioClips sfx;
    private bool soundPlayed = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = FindObjectOfType<PlayerMove>().transform;
        agent = GetComponent<NavMeshAgent>();
        spawnPoint = transform.position;
        currentPatrolIndex = 0;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInSight = IsPlayerInSight();

        switch (currentState)
        {
            case State.Patrolling:
                if (playerInSight && distanceToPlayer <= detectionRange)
                {
                    animator.SetBool("isChasing", true);
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
                        sfx.PlayOneShot("GoblinChase");
                        soundPlayed = true;
                    }


                    agent.SetDestination(player.position);

                    if (distanceToPlayer <= bombThrowRange)
                    {
                        currentState = State.ThrowingBomb;
                    }
                }

                if (Time.time - lastSeenTime > searchDuration)
                {
                    animator.SetBool("isChasing", false);
                    soundPlayed = false;
                    currentState = State.Searching;
                }
                break;

            case State.Searching:
                agent.SetDestination(spawnPoint);
                if (Vector3.Distance(transform.position, spawnPoint) < 1f)
                {
                    currentState = State.Patrolling;
                    GoToNextPatrolPoint();
                }
                break;

            case State.ThrowingBomb:
                agent.isStopped = true;
                if (Time.time >= lastThrowTime + throwCooldown)
                {
                    ThrowBomb();
                }
                if (distanceToPlayer > bombThrowRange)
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

    private void ThrowBomb()
    {
        animator.SetTrigger("Throw");
        sfx.PlayOneShot("GoblinThrow");
        lastThrowTime = Time.time;

        if (bombPrefab != null && bombSpawnPoint != null)
        {
            GameObject bomb = Instantiate(bombPrefab, bombSpawnPoint.position, Quaternion.identity);
            Bomb bombScript = bomb.GetComponent<Bomb>();
            if (bombScript != null)
            {
                bombScript.Initialize(bombExplosionRadius, bombDamage, player.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bombThrowRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
