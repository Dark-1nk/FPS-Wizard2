using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coins : MonoBehaviour
{
    // UI Canvas to track items
    public TMP_Text money; // Assign the canvas in the inspector

    private bool playerInRange = false; // Tracks if the player is in range
    private PlayerMove playerMove; // Reference to PlayerMove component

    void Start()
    {
        money.text = "$0";
    }

    void Update()
    {
        // Check if player is in range and presses E
        if (playerInRange)
        {
            playerMove.money++;
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
        money.text = ("$" + playerMove.money + "B");
        Destroy(gameObject);
    }
}
