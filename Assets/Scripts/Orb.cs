using UnityEngine;
using UnityEngine.UI;

public class Orb : MonoBehaviour
{
    // Boolean selectors for item types
    public bool isRed;
    public bool isOrange;
    public bool isYellow;
    public bool isGreen;
    public bool isBlue;
    public bool isViolet;
    public bool isPink;

    DoorManager doorManager;

    // UI Canvas to track items
    public GameObject orbVisual; // Assign the canvas in the inspector
    public CanvasManager canvasManager;

    private bool playerInRange = false; // Tracks if the player is in range
    private PlayerMove playerMove; // Reference to PlayerMove component

    void Start()
    {
        if (orbVisual != null)
        {
            orbVisual.SetActive(false); // Start with the canvas inactive
        }

        doorManager = FindObjectOfType<DoorManager>();
    }

    void Update()
    {
        // Check if player is in range and presses E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            playerMove.orbsCollected++;
            GetItemColor();
            OnPickUp();
            PickUpItem();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a PlayerMove
        playerMove = other.GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset when the player exits the trigger
        if (other.GetComponent<PlayerMove>() != null)
        {
            playerInRange = false;
        }
    }

    private void PickUpItem()
    {
        // Update the UI to show the collected item
        if (orbVisual != null)
        {
            orbVisual.SetActive(true); // Show the orb
        }

        Destroy(gameObject);
    }

    private void GetItemColor()
    {
        // Determine the item's color based on the booleans
        if (isRed) playerMove.hasRed = true;
        if (isOrange) playerMove.hasOrange = true;
        if (isYellow) playerMove.hasYellow = true; ;
        if (isGreen) playerMove.hasGreen = true;
        if (isBlue) playerMove.hasBlue = true;
        if (isViolet) playerMove.hasPurple = true;
        if (isPink) playerMove.hasPink = true;
    }

    private void OnPickUp()
    {
        if (isRed)
        {
            canvasManager.OpenCanvas("RedInfo", true);
            doorManager.OpenMe("Red");
        }
        if (isOrange)
        {
            canvasManager.OpenCanvas("OrangeInfo", true);
        }
        if (isYellow)
        {
            canvasManager.OpenCanvas("YellowInfo", true);
            playerMove.GetComponentInChildren<SparkBolt>().fireRate = 0.5f;
        }
        if (isGreen)
        {
            canvasManager.OpenCanvas("GreenInfo", true);
            doorManager.OpenMe("Green");
            playerMove.health = 3;
        }
        if (isBlue)
        {
            canvasManager.OpenCanvas("BlueInfo", true);
            playerMove.maxJumps = 2;
            playerMove.momentumDamping = 0.3f;
        }
        if (isViolet)
        {
            canvasManager.OpenCanvas("PurpleInfo", true);
            playerMove.health = 1;
        }
        if (isPink)
        {
            canvasManager.OpenCanvas("PinkInfo", true);
            doorManager.OpenMe("Pink");
            playerMove.GetComponentInChildren<SparkBolt>().bigDamage = 3;
            playerMove.GetComponentInChildren<SparkBolt>().smallDamage = 0.5f;
        }
    }
}
