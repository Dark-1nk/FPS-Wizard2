using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [System.Serializable]
    public class CanvasEntry
    {
        public string canvasName;
        public GameObject canvasObject;
    }

    public List<CanvasEntry> canvases; // List of canvases and their names
    public GameObject defaultCanvas; // Default canvas to reset to
    public KeyCode closeCanvasKey = KeyCode.E; // Key to close canvas and reset to default
    private GameObject currentCanvas; // Tracks the currently active canvas

    void Start()
    {
        // Disable all canvases initially except the default one
        foreach (var canvas in canvases)
        {
            if (canvas.canvasObject != null)
                canvas.canvasObject.SetActive(false);
        }

        if (defaultCanvas != null)
        {
            OpenCanvas(defaultCanvas.name, false); // Open the default canvas without pausing the game
        }
    }

    void Update()
    {
        // Close current canvas and reset to default on key press
        if (Input.GetKeyDown(closeCanvasKey))
        {
            CloseCanvas();
        }
    }

    public void OpenCanvas(string canvasName, bool pausesGame)
    {
        // Close all canvases first
        foreach (var canvas in canvases)
        {
            if (canvas.canvasObject != null)
                canvas.canvasObject.SetActive(false);
        }

        // Find and open the specified canvas
        var targetCanvas = canvases.Find(c => c.canvasName == canvasName);
        if (targetCanvas != null && targetCanvas.canvasObject != null)
        {
            targetCanvas.canvasObject.SetActive(true);
            currentCanvas = targetCanvas.canvasObject;

            // Pause the game if specified
            Time.timeScale = pausesGame ? 0 : 1;
        }
        else
        {
            Debug.LogWarning($"Canvas with name '{canvasName}' not found in the list.");
        }
    }

    public void CloseCanvas()
    {
        // Disable the current canvas
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false);
            currentCanvas = null;
        }

        // Reset to the default canvas
        if (defaultCanvas != null)
        {
            defaultCanvas.SetActive(true);
            currentCanvas = defaultCanvas;

            // Ensure the game is unpaused when returning to default canvas
            Time.timeScale = 1;
        }
        else
        {
            Debug.LogWarning("Default canvas is not assigned.");
        }
    }
}
