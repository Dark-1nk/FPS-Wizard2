using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GhostAI : MonoBehaviour
{
    public Transform player;
    public float spawnDistanceBehindPlayer = 20f;
    public float minSpawnInterval = 15f;
    public float maxSpawnInterval = 30f;
    public float creepSpeed = 1f;
    public float fastSpeed = 5f;
    public float disappearanceDelay = 1f;
    public float killRange = 1f;
    public AudioClips sfx;
    public AudioClips music;

    public Camera playerCamera;
    public CanvasManager canvasManager;

    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool hasBeenSeen = false;
    private bool isSpawned = false;
    private bool isDisappearing = false;

    private enum State { Idle, Creeping, Seen, Disappearing }
    private State currentState = State.Idle;

    private float reappearTimer = 0f;
    private bool isDisabled = false;

    void Start()
    {
        //may need to initialize hasPlayed here


        player = FindObjectOfType<PlayerMove>().transform;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(SpawnAtRandomInterval());
    }

    void Update()
    {
        if (player.GetComponent<PlayerMove>().isDead)
        {
            return;
        }

        if (!isSpawned || isDisabled) return;
        animator.SetBool("isSeen", hasBeenSeen);
        bool isLookingAtEnemy = IsPlayerLookingAtEnemy();

        switch (currentState)
        {
            case State.Creeping:
                agent.speed = creepSpeed;
                agent.SetDestination(player.position);

                if (isLookingAtEnemy)
                {
                    hasBeenSeen = true;
                    currentState = State.Seen;
                }
                break;

            case State.Seen:
                agent.speed = fastSpeed;
                agent.SetDestination(player.position);

                if (!isLookingAtEnemy && hasBeenSeen)
                {
                    currentState = State.Disappearing;
                    isDisappearing = true;
                    reappearTimer = 0f; // Reset timer for disappearance
                }
                break;

            case State.Disappearing:
                if (isDisappearing)
                {
                    reappearTimer += Time.deltaTime;

                    if (reappearTimer >= disappearanceDelay)
                    {
                        isDisappearing = false;
                        DisableGhost();
                        StartCoroutine(RespawnGhost());
                    }
                }
                break;
        }

        CheckForKill();
    }

    private IEnumerator SpawnAtRandomInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            SpawnBehindPlayer();
        }
    }

    private void SpawnBehindPlayer()
    {
        Vector3 spawnPosition = player.position - player.forward * spawnDistanceBehindPlayer;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(spawnPosition, out navHit, 5f, NavMesh.AllAreas))
        {
            spawnPosition = navHit.position; // Adjust to the nearest valid NavMesh point
        }

        transform.position = spawnPosition;
        isSpawned = true;
        currentState = State.Creeping;

        // Re-enable ghost visuals and behavior
        EnableGhost();
    }

    private void DisableGhost()
    {
        isDisabled = true;
        isSpawned = false;

        // Disable visuals and physics
        if (animator != null)
            animator.enabled = false;
        if (agent != null)
            agent.isStopped = true;
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    private void EnableGhost()
    {
        isDisabled = false;
        isSpawned = true;

        // Enable visuals and physics
        if (animator != null)
            animator.enabled = true;
        if (agent != null)
            agent.isStopped = false;
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
        sfx.PlayOneShot("Ghost");
    }

    private IEnumerator RespawnGhost()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
        SpawnBehindPlayer();
    }

    private bool IsPlayerLookingAtEnemy()
    {
        Vector3 directionToEnemy = (transform.position - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToEnemy);

        // Check if the enemy is within the camera's field of view (e.g., 60 degrees)
        return angle < playerCamera.fieldOfView;
    }

    void CheckForKill()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Kill logic
        if (!player.GetComponent<PlayerMove>().isDead && distanceToPlayer <= killRange)
        {
            agent.isStopped = true;
            KillPlayer();
            player.GetComponent<PlayerMove>().isDead = true;
            return;
        }
    }

    private void KillPlayer()
    {
        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            Debug.Log("Ghost has killed player");

            music.StopClip(0);
            sfx.StopClip(0);
            canvasManager.OpenCanvas("GhostDeath", false);
            sfx.PlayOneShot("GhostDeath");

            StartCoroutine(DeathSequence());
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);
    }
    private IEnumerator DeathSequence()
    {
        Debug.Log("Player has died. Changing scene in 2 seconds...");

        // Wait for 2 seconds
        float elapsedTime = 0f;
        while (elapsedTime < 2f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Load the game over scene
        SceneManager.LoadScene("Game Over");
    }
}

