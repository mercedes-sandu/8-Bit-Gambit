using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private KeyCode inputKey = KeyCode.Space;
    
    [SerializeField] private float keyTapThreshold = 0.5f;
    [SerializeField] private float keyHoldThreshold = 2f;
    
    // note: these are not actual buttons because the player cannot click them anyway
    // we will simulate the clicks
    [SerializeField] private SpriteRenderer startButton;
    [SerializeField] private SpriteRenderer quitButton;

    [SerializeField] private Sprite buttonNormal;
    [SerializeField] private Sprite buttonHighlighted;
    [SerializeField] private Sprite buttonPressed;

    private SpriteRenderer _startButtonProgressBar;
    private SpriteRenderer _quitButtonProgressBar;

    private Animator _startButtonProgressBarAnimator;
    private Animator _quitButtonProgressBarAnimator;
    
    private bool _isKeyDown = false;
    private float _keyTapTimer;
    private float _keyHoldTimer;

    private List<(SpriteRenderer, SpriteRenderer, Animator)> _buttons = new();
    private (SpriteRenderer, SpriteRenderer, Animator) _selectedButton;
    private int _selectedButtonIndex;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        // _startButtonProgressBar = startButton.transform.GetChild(0).GetComponent<SpriteRenderer>();
        // _quitButtonProgressBar = quitButton.transform.GetChild(0).GetComponent<SpriteRenderer>();
        //
        // _startButtonProgressBarAnimator = startButton.GetComponent<Animator>();
        // _quitButtonProgressBarAnimator = quitButton.GetComponent<Animator>();
        //
        // startButton.sprite = buttonHighlighted;
        // quitButton.sprite = buttonNormal;
        //
        // _buttons = new List<(SpriteRenderer, SpriteRenderer, Animator)>()
        // {
        //     (startButton, _startButtonProgressBar, _startButtonProgressBarAnimator),
        //     (quitButton, _quitButtonProgressBar, _quitButtonProgressBarAnimator)
        // };
        // _selectedButton = (startButton, _startButtonProgressBar, _startButtonProgressBarAnimator);
        // _selectedButtonIndex = 0;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        // if (_isKeyDown && _keyTapTimer <= 0f)
        // {
        //     _keyHoldTimer -= Time.deltaTime;
        // }
        //
        // if (_isKeyDown && _keyTapTimer > 0f)
        // {
        //     _keyTapTimer -= Time.deltaTime;
        // }
        //
        // if (_keyHoldTimer < keyHoldThreshold && _keyHoldTimer > 0f)
        // {
        //     HoldInput();
        // }
        //
        // if (_keyHoldTimer <= 0f)
        // {
        //     _isKeyDown = false;
        // }
        //
        // if (Input.GetKeyDown(inputKey))
        // {
        //     _isKeyDown = true;
        // }
        //
        // if (!Input.GetKeyUp(inputKey)) return;
        //
        // if (_isKeyDown)
        // {
        //     if (_keyTapTimer > 0f)
        //     {
        //         TapInput();
        //     }
        //     if (_keyHoldTimer >= 0f)
        //     {
        //         ReleaseInput();
        //     }
        // }
        //     
        // _keyTapTimer = keyTapThreshold;
        // _keyHoldTimer = keyHoldThreshold;
        // _isKeyDown = false;
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
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void ReleaseInput()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartButton()
    {
        // load the next scene... instructions scene?
    }

    /// <summary>
    /// 
    /// </summary>
    private void QuitButton()
    {
        Application.Quit();
    }
}