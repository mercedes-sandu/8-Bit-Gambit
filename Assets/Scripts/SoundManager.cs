using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource _audioSource;
    public AudioClip _selectPieceClip;
    public AudioClip[] _explosionSoundClips;

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

    public void PlaySelect()
    {
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(_selectPieceClip);
        }
    }

    public void PlayExplosion()
    {
        if (_audioSource != null)
        {
            int randMax = _explosionSoundClips.Length;
            if (randMax > 0)
            {
                int index = Random.Range(0, randMax);
                _audioSource.PlayOneShot(_explosionSoundClips[index], 0.8f);
            }
        }
    }
}
