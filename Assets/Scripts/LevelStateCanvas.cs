using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelStateCanvas : MonoBehaviour
{
    [SerializeField] private float keyTapThreshold = 0.5f;
    [SerializeField] private float keyHoldThreshold = 2f;
    
    [SerializeField] private Sprite buttonHighlighted;
    [SerializeField] private Sprite buttonPressed;

    [SerializeField] private Image buttonImage;
    [SerializeField] private Image progressBarImage;

    private KeyCode _inputKey;

    private bool _canvasEnabled;
    
    private Sprite[] _progressBarSprites;
    private int _numSteps;

    private bool _isKeyDown = false;
    private float _keyTapTimer;
    private float _keyHoldTimer;

    private Coroutine _currentCoroutine;
    private bool _coroutineStarted = false;
    
    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        _inputKey = LevelManager.Instance ? LevelManager.Instance.inputKey : KeyCode.Space;

        _canvasEnabled = GetComponent<Canvas>().enabled;

        buttonImage.sprite = buttonHighlighted;
        
        progressBarImage.enabled = false;
        
        _progressBarSprites = 
            Resources.LoadAll<Sprite>("Sprites/Menu Button Progress Bars/Menu Button Progress Bar");
        _numSteps = _progressBarSprites.Length;
        
        _keyHoldTimer = keyHoldThreshold;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (!_canvasEnabled) return;
        
        if (_isKeyDown && _keyTapTimer <= 0f)
        {
            _keyHoldTimer -= Time.deltaTime;
        }
        
        if (_isKeyDown && _keyTapTimer > 0f)
        {
            _keyTapTimer -= Time.deltaTime;
        }
        
        if (_keyHoldTimer < keyHoldThreshold && _keyHoldTimer > 0f)
        {
            HoldInput();
        }
        
        if (_keyHoldTimer <= 0f)
        {
            _isKeyDown = false;
        }
        
        if (Input.GetKeyDown(_inputKey))
        {
            _isKeyDown = true;
        }
        
        if (!Input.GetKeyUp(_inputKey)) return;
        
        if (_isKeyDown)
        {
            if (_keyTapTimer > 0f)
            {
                TapInput();
            }
            if (_keyHoldTimer >= 0f)
            {
                ReleaseInput();
            }
        }
            
        _keyTapTimer = keyTapThreshold;
        _keyHoldTimer = keyHoldThreshold;
        _isKeyDown = false;
    }

    private void TapInput()
    {
        // do nothing
    }

    /// <summary>
    /// 
    /// </summary>
    private void HoldInput()
    {
        if (_coroutineStarted) return;

        _coroutineStarted = true;
        buttonImage.sprite = buttonPressed;
        progressBarImage.sprite = _progressBarSprites[0];
        progressBarImage.enabled = true;
        SoundManager.Instance.StartHoldAudio(keyHoldThreshold, _numSteps);
        _currentCoroutine = StartCoroutine(ProgressBar(0));
    }

    /// <summary>
    /// 
    /// </summary>
    private void ReleaseInput()
    {
        if (!_coroutineStarted) return;
        
        SoundManager.Instance.StopHoldAudio();
        StopCoroutine(_currentCoroutine);
        buttonImage.sprite = buttonHighlighted;
        progressBarImage.sprite = _progressBarSprites[0];
        progressBarImage.enabled = false;
        _coroutineStarted = false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="progressNumber"></param>
    /// <returns></returns>
    private IEnumerator ProgressBar(int progressNumber)
    {
        progressBarImage.sprite = _progressBarSprites[progressNumber];
        
        yield return new WaitForSeconds(keyHoldThreshold / _numSteps);
        
        progressNumber++;
        if (progressNumber < _numSteps)
        {
            yield return ProgressBar(progressNumber);
        }
        else
        {
            SoundManager.Instance.StopHoldAudio();
            progressBarImage.sprite = _progressBarSprites[0];
            progressBarImage.enabled = false;
            _coroutineStarted = false;
            LoadLevel();
        }
    }
    
    /// <summary>
    /// Called by "buttons" in levelWonCanvas, levelLostCanvas, and levelDrawCanvas to load the appropriate next level
    /// or reload the current level.
    /// </summary>
    private void LoadLevel()
    {
        // this is guaranteed to know which level to load at the time of calling
        GameMaster.LoadLevel();
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableCanvasInput()
    {
        _canvasEnabled = true;
    }
}