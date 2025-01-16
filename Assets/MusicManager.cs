using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [Header("Scene and Audio Clip Mapping")]
    public List<SceneMusic> sceneMusicList; // List of scenes and their corresponding audio clips
    private AudioClips audioClips; // Reference to your AudioClips script

    private string currentSceneName = ""; // Keeps track of the current scene to prevent redundant music changes

    void Awake()
    {
        // Ensure this object persists across scenes
        DontDestroyOnLoad(gameObject);

        // Get the reference to the AudioClips script
        audioClips = GetComponent<AudioClips>();

        // Register for scene loaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Unregister from scene loaded events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the new scene has a specific audio clip assigned
        string newSceneName = scene.name;

        // Avoid changing the music if it's already playing for the current scene
        if (newSceneName == currentSceneName) return;

        currentSceneName = newSceneName;

        // Find the matching audio clip for the loaded scene
        foreach (SceneMusic sceneMusic in sceneMusicList)
        {
            if (sceneMusic.sceneName == newSceneName)
            {
                // Play the corresponding music
                audioClips.PlayClipLoop(sceneMusic.audioClipName);
                return;
            }
        }

        // If no specific music is assigned for this scene, stop music
        audioClips.StopClip();
    }
}

[System.Serializable]
public class SceneMusic
{
    public string sceneName; // Name of the scene
    public string audioClipName; // Name of the audio clip to play (used by your AudioClips script)
}
