using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public float playerSpeed = 10f;
    private CharacterController myCC;
    public float momentumDamping = 5f;
    public AudioClips sfx;

    public int maxHealth = 3;
    public int health;

    public Animator vignette;
    public Animator camAnim;
    public bool isWalking;
    public WandAnimator wand;
    public Animator heartsAnim;

    public bool isDead;

    [Header("Orbs")]
    public int orbsCollected = 0;
    public bool hasRed = false;
    public bool hasOrange = false;
    public bool hasYellow = false;
    public bool hasGreen = false;
    public bool hasBlue = false;
    public bool hasPurple = false;
    public bool hasPink = false;

    public int money;
    private Vector3 inputVector;
    private Vector3 movementVector;
    private readonly float myGravity = -10f;
    private float verticalVelocity = 0f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f; // How high the player can jump
    public float gravityMultiplier = 2f; // Adjust gravity for better jump physics
    public float groundCheckTime = 0.2f; // Time buffer for grounded state
    private float groundCheckTimer = 0f;

    public int maxJumps = 1;
    private int jumps;

    void Start()
    {
        isDead = false;
        health = maxHealth;
        myCC = GetComponent<CharacterController>();
    }

    void Update()
    {
        GetInput();
        MovePlayer();
        Die();
        heartsAnim.SetInteger("Health", health);
        vignette.SetInteger("Health", health);

        camAnim.SetBool("isWalking", isWalking);
    }

    void GetInput()
    {
        // Movement input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            inputVector.Normalize();
            inputVector = transform.TransformDirection(inputVector);

            isWalking = true;
        }
        else
        {
            inputVector = Vector3.MoveTowards(inputVector, Vector3.zero, momentumDamping * Time.deltaTime);

            isWalking = false;
        }

        // Check grounded state with a timer buffer
        if (myCC.isGrounded)
        {
            groundCheckTimer = groundCheckTime; // Reset the timer when grounded
        }
        else
        {
            groundCheckTimer -= Time.deltaTime; // Count down when not grounded
        }

        bool isGroundedWithBuffer = groundCheckTimer > 0f;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isGroundedWithBuffer || jumps > 0)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * (myGravity * gravityMultiplier));
                sfx.PlayOneShot("PlayerJump"); // Play jump sound
                jumps--;
            }
        }

        // Apply gravity
        if (!myCC.isGrounded)
        {
            verticalVelocity += myGravity * gravityMultiplier * Time.deltaTime;
        }
        else if (verticalVelocity < 0f)
        {
            verticalVelocity = 0f; // Reset vertical velocity when grounded
            jumps = maxJumps;
        }

        movementVector = (inputVector * playerSpeed) + (Vector3.up * verticalVelocity);
    }


    public void TakeDamage(int damage)
    {
        sfx.PlayOneShot("PlayerDamage");
        health -= damage;
        wand.Hurt();
    }

    public void Die()
    {
        if (health <= 0)
        {
            isDead = true;
            SceneManager.LoadScene("Game Over");
        }
    }

    void MovePlayer()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            myCC.Move(2 * Time.deltaTime * movementVector);
        }
        else
        {
            myCC.Move(movementVector * Time.deltaTime);
        }
    }
}
