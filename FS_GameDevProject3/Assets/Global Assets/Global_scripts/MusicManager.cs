using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Singleton Instance
    public static MusicManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip chaseMusic;

    private bool isChaseMusicPlaying = false;

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
        PlayBackgroundMusic(); // Play background music on start
    }

    // Method to play background music
    public void PlayBackgroundMusic()
    {
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Method to switch to chase music
    public void PlayChaseMusic()
    {
        if (!isChaseMusicPlaying || audioSource.clip != chaseMusic)
        {
            Debug.Log("Switching to Chase Music"); // Debug log for chase music
            audioSource.clip = chaseMusic;
            audioSource.loop = true;
            audioSource.Play();
            isChaseMusicPlaying = true;
        }
    }

    // Method to stop chase music and return to background music
    public void StopChaseMusic()
    {
        if (isChaseMusicPlaying)
        {
            Debug.Log("Switching to Chase Music"); // Debug log for chase music
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
            isChaseMusicPlaying = false;
        }
    }

    // Method to pause the music
    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
}
