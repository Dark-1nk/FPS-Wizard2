using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public Image visual;  // Image component to display the ending visual
    public TMP_Text title;    // Text component for the title
    public TMP_Text body;     // Text component for the body text

    [Header("Ending Content")]
    public Sprite[] visuals;  // Array of visuals corresponding to different endings
    public string[] titles;   // Array of titles corresponding to different endings

    [TextArea(3, 10)]  // Multi-line text area for longer body texts
    public string[] bodies;   // Array of body texts corresponding to different endings

    public float timeStarted;
    public float timeUntilClick = 5f;

    void Start()
    {
        timeStarted = Time.time;
        // Access the player stats from the GameManager
        int orbs = GameManager.Instance.orbsCollected;
        int money = GameManager.Instance.money;

        // Determine the ending index based on the player's stats
        int endingIndex = DetermineEndingIndex(orbs, money);

        // Display the corresponding ending
        DisplayEnding(endingIndex);
    }

    private void Update()
    {
        if (timeStarted + timeUntilClick >= Time.time)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
    /// <summary>
    /// Determines the ending index based on player stats.
    /// </summary>
    private int DetermineEndingIndex(int orbs, int money)
    {
        if (GameManager.Instance != null)
        {
            if (money == 20)
            {
                Debug.Log("You got the rich ending!");
                return 0;
            }
            else if (orbs == 7 && money < 20)
            {
                Debug.Log("You got the good ending!");
                return 1;
            }
            else
            {
                Debug.Log("You got the bad ending...");
                return 2;
            }
        }
        else
        {
            Debug.Log("You didn't get here the right way...");
            return 2;
        }
    }

    /// <summary>
    /// Displays the visual, title, and body text for the given ending index.
    /// </summary>
    private void DisplayEnding(int index)
    {
        // Ensure the index is within bounds to avoid errors
        if (index < 0 || index >= visuals.Length || index >= titles.Length || index >= bodies.Length)
        {
            Debug.LogError("Invalid ending index! Please check your visuals, titles, and bodies arrays.");
            return;
        }

        // Set the UI components to display the corresponding ending content
        visual.sprite = visuals[index];
        title.text = titles[index];
        body.text = bodies[index];
    }
}
