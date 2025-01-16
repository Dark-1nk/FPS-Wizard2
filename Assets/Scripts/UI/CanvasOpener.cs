using System.Collections.Generic;
using UnityEngine;

public class CanvasOpener : MonoBehaviour
{
    [System.Serializable]
    public class CanvasEntry
    {
        public string canvasName; // Name identifier for the canvas
        public GameObject canvasObject; // Reference to the canvas GameObject
    }

    public List<CanvasEntry> canvases; // List of canvases
    private int currentIndex = 0; // Current canvas index
    public int nextCanvasIndex = 0; // Index of the next canvas to open

    private void Start()
    {
        // Initialize all canvases to inactive except the first
        for (int i = 0; i < canvases.Count; i++)
        {
            if (canvases[i].canvasObject != null)
            {
                canvases[i].canvasObject.SetActive(i == currentIndex);
            }
        }
    }

    public void OpenNextCanvas()
    {
        if (canvases.Count == 0)
        {
            Debug.LogWarning("No canvases available to manage.");
            return;
        }

        // Deactivate the current canvas
        if (canvases[currentIndex].canvasObject != null)
        {
            canvases[currentIndex].canvasObject.SetActive(false);
        }

        // Activate the next canvas
        currentIndex = nextCanvasIndex % canvases.Count;
        if (canvases[currentIndex].canvasObject != null)
        {
            canvases[currentIndex].canvasObject.SetActive(true);
        }

        // Update the next canvas index
        nextCanvasIndex = (currentIndex + 1) % canvases.Count;
    }

    public void OpenCanvasByName(string canvasName)
    {
        int index = canvases.FindIndex(c => c.canvasName == canvasName);
        if (index == -1)
        {
            Debug.LogWarning($"Canvas with name '{canvasName}' not found.");
            return;
        }

        // Deactivate the current canvas
        if (canvases[currentIndex].canvasObject != null)
        {
            canvases[currentIndex].canvasObject.SetActive(false);
        }

        // Activate the specified canvas
        currentIndex = index;
        if (canvases[currentIndex].canvasObject != null)
        {
            canvases[currentIndex].canvasObject.SetActive(true);
        }

        // Update the next canvas index
        nextCanvasIndex = (currentIndex + 1) % canvases.Count;
    }

    public void ResetToFirstCanvas()
    {
        OpenCanvasByName(canvases[0].canvasName);
    }
}
