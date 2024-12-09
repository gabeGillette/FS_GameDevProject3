using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; } // Singleton instance

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip chaseMusic;

    private void Awake()
    {
        // Ensure only one instance of MusicManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this manager across scenes
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void SwitchToChaseMusic()
    {
        if (audioSource.clip != chaseMusic)
        {
            audioSource.clip = chaseMusic;
            audioSource.Play();
        }
    }

    public void SwitchToBackgroundMusic()
    {
        if (audioSource.clip != backgroundMusic)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }

    private void PlayBackgroundMusic()
    {
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

    // Method to pause music
    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
}