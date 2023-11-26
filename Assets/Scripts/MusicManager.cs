using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance = null;
    
    [SerializeField] private AudioClip menuTrack;
    [SerializeField] private AudioClip levelCompleteTrack;
    [SerializeField] private AudioClip gameOverTrack;
    [SerializeField] private AudioClip victoryTrack;
    [SerializeField] private AudioClip levelTrack;
    
    private AudioSource _audioSource;

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
        
        _audioSource = GetComponent<AudioSource>();
        PlayLevelMusic();
    }

    public void PlayMenuMusic()
    {
        _audioSource.loop = true;
        _audioSource.clip = menuTrack;
        _audioSource.volume = 0.25f;
        _audioSource.Play();
    }

    public void PlayLevelCompleteMusic()
    {
        _audioSource.loop = false;
        _audioSource.clip = levelCompleteTrack;
        _audioSource.volume = 0.4f;
        _audioSource.PlayDelayed(1f);
    }

    private void PlayLevelMusic()
    {
        _audioSource.clip = levelTrack;
        _audioSource.loop = true;
        _audioSource.volume = 0.25f;
        _audioSource.Play();
    }

    public void PlayGameOverMusic()
    {
        _audioSource.loop = true;
        _audioSource.clip = gameOverTrack;
        _audioSource.volume = 0.25f;
        _audioSource.PlayDelayed(1f);
    }
}