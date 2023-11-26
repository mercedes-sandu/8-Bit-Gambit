using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;
    
    [SerializeField] private AudioClip selectPieceClip;
    [SerializeField] private AudioClip holdConfirmClip;
    [SerializeField] private AudioClip[] explosionSoundClips;
    [SerializeField] private AudioClip airHornClip;
    [SerializeField] private float delayBeforeAirHorn = 0.2f;

    private AudioSource _audioSource;
    private Coroutine _currentCoroutine;
    private bool _coroutineStarted = false;
    private float _timeToHold;
    private float _startingVolume;

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
    }

    public void PlaySelect()
    {
        if (_audioSource) _audioSource.PlayOneShot(selectPieceClip, 0.6f);
    }

    public void PlayExplosion()
    {
        if (!_audioSource) return;
        
        var randMax = explosionSoundClips.Length;
        if (randMax <= 0) return;
        var index = Random.Range(0, randMax);
        _audioSource.PlayOneShot(explosionSoundClips[index], 0.8f);
        Invoke(nameof(PlayAirHorn), delayBeforeAirHorn);
    }

    private void PlayAirHorn()
    {
        _audioSource.PlayOneShot(airHornClip, 0.8f);
    }

    private IEnumerator RaiseAudio(int progressNum, int numSteps)
    {
        var increment = Mathf.Lerp(0, 1, _timeToHold / numSteps);
        _audioSource.pitch += increment / 4;
        yield return new WaitForSeconds(_timeToHold / numSteps);

        progressNum++;
        if (progressNum < numSteps)
        {
            yield return RaiseAudio(progressNum, numSteps);
        }
    }

    public void StartHoldAudio(float timeToHold, int numSteps)
    {
        if (_coroutineStarted) return;
        
        _startingVolume = _audioSource.volume;
        _audioSource.volume = 0.35f;
        _audioSource.pitch = 0.5f;
        _timeToHold = timeToHold;
        _coroutineStarted = true;
        _audioSource.loop = true;
        _audioSource.clip = holdConfirmClip;
        _audioSource.Play();
        _currentCoroutine = StartCoroutine(RaiseAudio(0, numSteps));
    }

    public void StopHoldAudio()
    {
        StopCoroutine(_currentCoroutine);
        _audioSource.Stop();
        _audioSource.volume = _startingVolume;
        _audioSource.loop = false;
        _audioSource.pitch = 1;
        _coroutineStarted = false;
    }
}