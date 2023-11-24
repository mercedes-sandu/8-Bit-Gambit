using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelStateCanvas : MonoBehaviour
{
    [SerializeField] private float keyHoldThreshold = 2f;

    [SerializeField] private Image progressBarImage;
    
    [SerializeField] private KeyCode inputKey = KeyCode.Space;

    private bool _canvasEnabled = false;
    
    private Sprite[] _progressBarSprites;
    private int _numSteps;

    private bool _isKeyDown = false;
    private float _keyHoldTimer;

    private Coroutine _currentCoroutine;
    private bool _coroutineStarted = false;
    
    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
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
        
        if (_isKeyDown && _keyHoldTimer <= 0f)
        {
            _keyHoldTimer -= Time.deltaTime;
        }
        
        if (_isKeyDown && _keyHoldTimer > 0f)
        {
            _keyHoldTimer -= Time.deltaTime;
        }
        
        if (_keyHoldTimer <= 0f && !_coroutineStarted)
        {
            HoldInput();
        }

        if (_keyHoldTimer <= 0f)
        {
            _isKeyDown = false;
        }

        if (Input.GetKeyDown(inputKey))
        {
            _isKeyDown = true;
        }

        if (!Input.GetKeyUp(inputKey)) return;

        if (_isKeyDown && _keyHoldTimer >= 0f) ReleaseInput();
        
        _keyHoldTimer = keyHoldThreshold;
        _isKeyDown = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void HoldInput()
    {
        if (_coroutineStarted) return;

        _coroutineStarted = true;
        progressBarImage.sprite = _progressBarSprites[0];
        progressBarImage.enabled = true;
        _currentCoroutine = StartCoroutine(ProgressBar(0));
    }

    /// <summary>
    /// 
    /// </summary>
    private void ReleaseInput()
    {
        if (!_coroutineStarted) return;
        
        StopCoroutine(_currentCoroutine);
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