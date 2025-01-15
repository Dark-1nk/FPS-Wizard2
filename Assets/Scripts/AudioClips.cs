using System.Collections;
using UnityEngine;

public class AudioClips : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component is missing from this GameObject.");
        }
    }

    public void PlayClip(string clipName)
    {
        if (audioSource == null) return;

        AudioClip clip = AudioManager.Instance.GetClip(clipName);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    public void PlayClipLoop(string clipName)
    {
        if (audioSource == null) return;

        AudioClip clip = AudioManager.Instance.GetClip(clipName);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void PlayOneShot(string clipName)
    {
        if (audioSource == null) return;

        AudioClip clip = AudioManager.Instance.GetClip(clipName);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void StopClip(float fadeDuration = 1f)
    {
        if (audioSource == null || !audioSource.isPlaying) return;

        StartCoroutine(FadeOutAndStop(fadeDuration));
    }

    private IEnumerator FadeOutAndStop(float duration)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
        audioSource.clip = null; // Clear the clip to ensure it's reset
        audioSource.volume = startVolume; // Reset volume for future use
    }
}