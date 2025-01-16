using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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

    public Camera playerCamera;
    public CanvasManager canvasManager;

    public Animator wiz;
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
        player = FindObjectOfType<PlayerMove>().transform;
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(SpawnAtRandomInterval());
    }

    void Update()
    {
        if (player.GetComponent<PlayerMove>().isDead)
        {
            return;
        }

        if (wiz != null)
        {
            wiz.SetBool("isSpawned", isSpawned);
        }

        if (!isSpawned || isDisabled) return;
        animator.SetBool("isSeen", hasBeenSeen);
        bool isLookingAtEnemy = IsPlayerLookingAtEnemy();

        switch (currentState)
        {
            case State.Creeping:
                MoveTowardsPlayer(creepSpeed);

                if (isLookingAtEnemy)
                {
                    hasBeenSeen = true;
                    currentState = State.Seen;
                }
                break;

            case State.Seen:
                MoveTowardsPlayer(fastSpeed);

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

    private void MoveTowardsPlayer(float speed)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
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

        transform.position = spawnPosition;
        spawnPosition.y = 3;
        isSpawned = true;
        currentState = State.Creeping;

        // Re-enable ghost visuals and behavior
        EnableGhost();
    }

    private void DisableGhost()
    {
        sfx.StopClip();
        isDisabled = true;
        isSpawned = false;

        // Disable visuals
        if (animator != null)
            animator.enabled = false;
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    private void EnableGhost()
    {
        isDisabled = false;
        isSpawned = true;

        // Enable visuals
        if (animator != null)
            animator.enabled = true;
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

            MusicManager.Instance.audioClips.StopClip(0);
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

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        // Load the game over scene
        SceneManager.LoadScene("Game Over");
    }
}
