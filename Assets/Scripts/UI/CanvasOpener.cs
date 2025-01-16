using System.Collections.Generic;
using UnityEngine;

public class CanvasOpener: MonoBehaviour
{
    [System.Serializable]
    public class CanvasEntry
    {
        public string canvasName; // Name identifier for the canvas
        public GameObject canvasObject; // Reference to the canvas GameObject
    }

    public AudioClips sfx;

    public List<CanvasEntry> canvases; // List of canvases
    private int currentIndex = -1; // Tracks the currently active canvas index

    /// <summary>
    /// Initializes and opens the first canvas in the list.
    /// </summary>
    public void OpenCanvas()
    {
        
        if (canvases.Count == 0)
        {
            Debug.LogWarning("No canvases available to manage.");
            return;
        }

        currentIndex = 0;
        ActivateCanvas(currentIndex);
    }

    /// <summary>
    /// Cycles to the next canvas in the list.
    /// </summary>
    public void NextCanvas()
    {
        if (canvases.Count == 0)
        {
            Debug.LogWarning("No canvases available to manage.");
            return;
        }

        // Deactivate the current canvas
        if (currentIndex != -1)
        {
            DeactivateCanvas(currentIndex);
        }

        // Cycle to the next canvas
        currentIndex = (currentIndex + 1) % canvases.Count;
        ActivateCanvas(currentIndex);
    }

    /// <summary>
    /// Activates the canvas at the specified index.
    /// </summary>
    /// <param name="index">Index of the canvas to activate.</param>
    private void ActivateCanvas(int index)
    {
        sfx.PlayOneShot("Click");
        if (index >= 0 && index < canvases.Count && canvases[index].canvasObject != null)
        {
            canvases[index].canvasObject.SetActive(true);
        }
    }

    /// <summary>
    /// Deactivates the canvas at the specified index.
    /// </summary>
    /// <param name="index">Index of the canvas to deactivate.</param>
    private void DeactivateCanvas(int index)
    {
        if (index >= 0 && index < canvases.Count && canvases[index].canvasObject != null)
        {
            canvases[index].canvasObject.SetActive(false);
        }
    }

    /// <summary>
    /// Deactivates all canvases.
    /// </summary>
    public void CloseAllCanvases()
    {
        sfx.PlayOneShot("EnemyDamage");
        foreach (var canvas in canvases)
        {
            if (canvas.canvasObject != null)
            {
                canvas.canvasObject.SetActive(false);
            }
        }
    }
}
