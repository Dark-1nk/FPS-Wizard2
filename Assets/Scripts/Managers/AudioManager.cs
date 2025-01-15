using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class AudioClipEntry
    {
        public string name;
        public AudioClip clip;
    }

    public List<AudioClipEntry> audioClips;

    private Dictionary<string, AudioClip> clipDictionary;

    void Awake()
    {
        // Singleton pattern to ensure a single instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeClipDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeClipDictionary()
    {
        clipDictionary = new Dictionary<string, AudioClip>();

        foreach (var entry in audioClips)
        {
            if (!clipDictionary.ContainsKey(entry.name))
            {
                clipDictionary.Add(entry.name, entry.clip);
            }
            else
            {
                Debug.LogWarning($"Duplicate audio clip name found: {entry.name}. Only the first one will be used.");
            }
        }
    }

    public AudioClip GetClip(string clipName)
    {
        if (clipDictionary.TryGetValue(clipName, out var clip))
        {
            return clip;
        }
        else
        {
            Debug.LogWarning($"Audio clip with name '{clipName}' not found.");
            return null;
        }
    }

    public void PlayClipOneShot(AudioSource source, string clipName)
    {
        var clip = GetClip(clipName);
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
        }
    }

    public void PlayClipLoop(AudioSource source, string clipName)
    {
        var clip = GetClip(clipName);
        if (clip != null && source != null)
        {
            source.clip = clip;
            source.loop = true;
            source.Play();
        }
    }
}
