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

    private Vector3[] patrolPoints;
    private int currentPatrolIndex;
    private Vector3 currentTarget; // To track the last destination

    private Animator animator;
    public AudioClips sfx;
    private bool soundPlayed = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = FindObjectOfType<PlayerMove>().transform;
        agent = GetComponent<NavMeshAgent>();
        spawnPoint = transform.position;

        GeneratePatrolPoints();
        currentPatrolIndex = 0;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInSight = IsPlayerInSight();

        if (currentState != State.ThrowingBomb) // Skip checks during bomb throwing
        {
            lastSeenTime = playerInSight ? Time.time : lastSeenTime;
        }

        switch (currentState)
        {
            case State.Patrolling:
                HandlePatrolling(playerInSight, distanceToPlayer);
                break;

            case State.Chasing:
                HandleChasing(playerInSight, distanceToPlayer);
                break;

            case State.Searching:
                HandleSearching();
                break;

            case State.ThrowingBomb:
                HandleThrowingBomb(distanceToPlayer);
                break;
        }
    }

    private void GeneratePatrolPoints()
    {
        patrolPoints = new Vector3[4];
        float patrolDistance = 20f; // Distance from the spawn point to each patrol point
        float sampleDistance = 2.0f; // NavMesh sample radius

        Vector3[] offsets = new Vector3[]
        {
        new Vector3(patrolDistance, 0, 0),  // Right
        new Vector3(-patrolDistance, 0, 0), // Left
        new Vector3(0, 0, patrolDistance),  // Forward
        new Vector3(0, 0, -patrolDistance)  // Backward
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3 targetPosition = spawnPoint + offsets[i];
            NavMeshHit hit;

            if (NavMesh.SamplePosition(targetPosition, out hit, sampleDistance, NavMesh.AllAreas))
            {
                patrolPoints[i] = hit.position;
            }
            else
            {
                Debug.LogWarning($"Failed to find a valid patrol point near {targetPosition}. Defaulting to spawn point.");
                patrolPoints[i] = spawnPoint; // Fallback to spawn point
            }
        }
    }


    private void HandlePatrolling(bool playerInSight, float distanceToPlayer)
    {
        if (playerInSight || distanceToPlayer <= detectionRange)
        {
            ChangeState(State.Chasing);
            animator.SetBool("isChasing", true);
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    private void HandleChasing(bool playerInSight, float distanceToPlayer)
    {
        if (playerInSight && distanceToPlayer <= chaseRange)
        {
            if (!soundPlayed)
            {
                sfx.PlayOneShot("GoblinChase");
                soundPlayed = true;
            }

            SetAgentDestination(player.position);

            if (distanceToPlayer <= bombThrowRange && Time.time >= lastThrowTime + throwCooldown)
            {
                ChangeState(State.ThrowingBomb);
            }
        }
        else
        {
            soundPlayed = false;

            if (!playerInSight || distanceToPlayer > chaseRange)
            {
                lastSeenTime = Time.time;
                ChangeState(State.Searching);
            }
        }
    }

    private void HandleSearching()
    {
        SetAgentDestination(spawnPoint);

        if (Vector3.Distance(transform.position, spawnPoint) < 1f)
        {
            ChangeState(State.Patrolling);
            GoToNextPatrolPoint();
        }
    }

    private void HandleThrowingBomb(float distanceToPlayer)
    {
        if (distanceToPlayer <= bombThrowRange && Time.time >= lastThrowTime + throwCooldown)
        {
            agent.isStopped = true;
            ThrowBomb();
        }
        else if (distanceToPlayer > bombThrowRange)
        {
            agent.isStopped = false;
            ChangeState(State.Chasing);
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        SetAgentDestination(patrolPoints[currentPatrolIndex]);
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
        Debug.Log("ThrowBomb method called.");
        animator.SetTrigger("Throw");
        sfx.PlayOneShot("GoblinThrow");
        lastThrowTime = Time.time;

        if (bombPrefab != null && bombSpawnPoint != null)
        {
            GameObject bomb = Instantiate(bombPrefab, bombSpawnPoint.position, Quaternion.identity);
            Debug.Log($"Bomb instantiated at {bombSpawnPoint.position}");

            Bomb bombScript = bomb.GetComponent<Bomb>();
            if (bombScript != null)
            {
                bombScript.Initialize(bombExplosionRadius, bombDamage, player.position);
                Debug.Log("Bomb script initialized.");
            }
            else
            {
                Debug.LogWarning("Bomb script missing on prefab.");
            }
        }
        else
        {
            Debug.LogWarning("Bomb prefab or spawn point not assigned.");
        }

        ChangeState(State.Chasing);
    }

    private void SetAgentDestination(Vector3 target)
    {
        if (currentTarget != target)
        {
            agent.SetDestination(target);
            currentTarget = target;
        }
    }

    private void ChangeState(State newState)
    {
        Debug.Log($"Transitioning from {currentState} to {newState}");
        currentState = newState;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bombThrowRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        if (patrolPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var point in patrolPoints)
            {
                Gizmos.DrawSphere(point, 0.5f);
            }
        }
    }
}

