using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene Images & Text")]
    public Image cutsceneImage;  // Reference to the UI Image component that will display the images
    public TMP_Text cutsceneText;    // Reference to the UI Text component to display text
    public Sprite[] cutsceneImages; // List of images to display during the cutscene
    public AudioClips sfx;

    [TextArea(3, 10)]  // This allows for a multi-line text area in the Inspector for longer text entries
    public string[] cutsceneTexts;  // List of texts to display during the cutscene

    private int currentIndex = 0; // Current index in the cutscene
    private bool isCutscenePlaying = true; // Flag to control cutscene flow
    private bool isTyping = false; // Flag to check if text is currently being typed
    private bool skipTyping = false; // Flag to indicate skipping the typing animation
    private float typingSpeed = 0.05f; // Typing speed (time between characters)

    [Header("Cutscene Settings")]
    public float clickDelay = 0.5f; // Delay between clicks to prevent fast forwarding

    void Start()
    {
        // Start with the first image and text
        currentIndex = 0;
        DisplayCutscene(currentIndex);
    }

    void Update()
    {
        // Check for player input to advance the cutscene (key or mouse button)
        if (isCutscenePlaying && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            sfx.PlayOneShot("Click");
            if (isTyping)
            {
                // If typing, skip the typing effect and show the full text immediately
                skipTyping = true;
            }
            else
            {
                // If not typing, advance to the next cutscene
                if (currentIndex >= cutsceneImages.Length - 1)
                {
                    EndCutscene();
                }
                else
                {
                    sfx.PlayOneShot("Click");
                    StartCoroutine(DelayBetweenClicks());
                }
            }
        }
    }

    // Method to display the current image and text
    void DisplayCutscene(int index)
    {
        // Make sure the index is valid
        if (index >= 0 && index < cutsceneImages.Length)
        {
            cutsceneImage.sprite = cutsceneImages[index];  // Set the current image
            cutsceneText.text = ""; // Clear the previous text
            StartCoroutine(TypingEffect(cutsceneTexts[index])); // Start typing effect for the current text
        }
    }

    // Coroutine to simulate the typing effect for text
    IEnumerator TypingEffect(string fullText)
    {
        isTyping = true;
        skipTyping = false;
        cutsceneText.text = ""; // Clear the text field before typing new text

        foreach (char letter in fullText)
        {
            if (skipTyping)
            {
                cutsceneText.text = fullText; // Show the full text immediately
                break;
            }

            cutsceneText.text += letter;  // Add one character at a time
            yield return new WaitForSeconds(typingSpeed);  // Wait for the typing speed
        }

        isTyping = false;
    }

    // Coroutine to handle the delay between clicks
    IEnumerator DelayBetweenClicks()
    {
        isCutscenePlaying = false;  // Pause input while the delay occurs
        yield return new WaitForSeconds(clickDelay);  // Wait for the defined delay time
        currentIndex++;
        DisplayCutscene(currentIndex);  // Show the next image and text
        isCutscenePlaying = true;  // Resume input
    }

    // End the cutscene and load the game scene
    void EndCutscene()
    {
        isCutscenePlaying = false;  // Disable cutscene interaction
        // Load the game scene (replace "GameScene" with the actual name of your game scene)
        SceneManager.LoadScene("Game");
    }
}
