using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource _audioSource;
    public AudioClip _menuTrack;
    public AudioClip _levelCompleteTrack;
    public AudioClip _gameOverTrack;
    public AudioClip _victoryTrack;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayMenuMusic()
    {
        _audioSource.loop = true;
        _audioSource.clip = _menuTrack;
        _audioSource.volume = 0.25f;
        _audioSource.Play();
    }

    public void PlayLevelCompleteMusic()
    {
        _audioSource.loop = false;
        _audioSource.clip = _levelCompleteTrack;
        _audioSource.volume = 0.4f;
        _audioSource.PlayDelayed(1f);
    }
}
