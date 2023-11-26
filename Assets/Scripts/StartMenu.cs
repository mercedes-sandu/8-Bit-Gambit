using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private KeyCode inputKey = KeyCode.Space;
    
    [SerializeField] private float keyTapThreshold = 0.5f;
    [SerializeField] private float keyHoldThreshold = 2f;
    
    // note: these are not actual buttons because the player cannot click them anyway
    // we will simulate the clicks
    [SerializeField] private Image startButton;
    [SerializeField] private Image quitButton;
    [SerializeField] private Image startButtonProgressBar;
    [SerializeField] private Image quitButtonProgressBar;
    
    [SerializeField] private Sprite buttonNormal;
    [SerializeField] private Sprite buttonHighlighted;
    [SerializeField] private Sprite buttonPressed;

    private Sprite[] _progressBarSprites;
    private int _numSteps;
    
    private bool _isKeyDown = false;
    private float _keyTapTimer;
    private float _keyHoldTimer;

    private Coroutine _currentCoroutine;
    private bool _coroutineStarted = false;

    private List<(Image, Image)> _buttons = new();
    private (Image, Image) _selectedButton;
    private int _selectedButtonIndex;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        Debug.Log(startButton);
        startButton.sprite = buttonHighlighted;
        quitButton.sprite = buttonNormal;

        _progressBarSprites = 
            Resources.LoadAll<Sprite>("Sprites/Menu Button Progress Bars/Menu Button Progress Bar");
        _numSteps = _progressBarSprites.Length;
        
        _buttons = new List<(Image, Image)>()
        {
            (startButton, startButtonProgressBar),
            (quitButton, quitButtonProgressBar)
        };
        _selectedButton = (startButton, startButtonProgressBar);
        _selectedButtonIndex = 0;
        
        _keyTapTimer = keyTapThreshold;
        _keyHoldTimer = keyHoldThreshold;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
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
        
        if (Input.GetKeyDown(inputKey))
        {
            _isKeyDown = true;
        }
        
        if (!Input.GetKeyUp(inputKey)) return;
        
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

    /// <summary>
    /// 
    /// </summary>
    private void TapInput()
    {
        _selectedButton.Item1.sprite = buttonNormal;
        _selectedButtonIndex = (_selectedButtonIndex + 1) % _buttons.Count;
        _selectedButton = _buttons[_selectedButtonIndex];
        _selectedButton.Item1.sprite = buttonHighlighted;
    }

    /// <summary>
    /// 
    /// </summary>
    private void HoldInput()
    {
        if (_coroutineStarted) return;

        _coroutineStarted = true;
        _selectedButton.Item1.sprite = buttonPressed;
        _selectedButton.Item2.sprite = _progressBarSprites[0];
        _selectedButton.Item2.enabled = true;
        _currentCoroutine = StartCoroutine(ProgressBar(0));
    }

    /// <summary>
    /// 
    /// </summary>
    private void ReleaseInput()
    {
        if (!_coroutineStarted) return;
        
        StopCoroutine(_currentCoroutine);
        _selectedButton.Item1.sprite = buttonHighlighted;
        _selectedButton.Item2.sprite = _progressBarSprites[0];
        _selectedButton.Item2.enabled = false;
        _coroutineStarted = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="progressNumber"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private IEnumerator ProgressBar(int progressNumber)
    {
        _selectedButton.Item2.sprite = _progressBarSprites[progressNumber];
        
        yield return new WaitForSeconds(keyHoldThreshold / _numSteps);
        
        progressNumber++;
        if (progressNumber < _numSteps)
        {
            yield return ProgressBar(progressNumber);
        }
        else
        {
            _selectedButton.Item2.sprite = _progressBarSprites[0];
            _selectedButton.Item2.enabled = false;
            
            switch (_selectedButtonIndex)
            {
                case 0:
                    StartButton();
                    break;
                case 1:
                    QuitButton();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartButton()
    {
        // todo: load instructions scene
        SceneManager.LoadScene("TestInput");
    }

    /// <summary>
    /// 
    /// </summary>
    private void QuitButton()
    {
        Application.Quit();
    }
}