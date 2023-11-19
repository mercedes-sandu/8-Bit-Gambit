using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InputState = LevelManager.InputState;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode inputKey = KeyCode.Space;
    
    [SerializeField] private float keyTapThreshold = 0.5f;
    [SerializeField] private float keyHoldThreshold = 2f;

    public GameObject PatternOverlayPrefab;
    private GameObject PatternOverlay;

    private const int NumSteps = 5;

    private List<ChessPiece> _pieces = new();
    private ChessPiece _selectedPlayerPiece;
    private int _selectedPlayerPieceIndex = 0;

    private List<ChessPiece> _targetPieces = new();
    private ChessPiece _selectedTargetPiece;
    private int _selectedTargetPieceIndex = 0;
    
    private bool _isKeyDown = false;
    private float _keyTapTimer;
    private float _keyHoldTimer;
    
    private Coroutine _currentCoroutine;
    private bool _coroutineStarted = false;

    /// <summary>
    /// Subscribes to game events.
    /// </summary>
    private void Awake()
    {
        GameEvent.OnConfirmSelectedPiece += ConfirmSelectedPiece;
        GameEvent.OnAttackTarget += Attack;
        _keyTapTimer = keyTapThreshold;
        _keyHoldTimer = keyHoldThreshold;
    }
    
    /// <summary>
    /// Get all the player's pieces and select the first one.
    /// </summary>
    private void Start()
    {
        _pieces = Board.Instance.GetPlayerPieces();
        _selectedPlayerPiece = _pieces[_selectedPlayerPieceIndex];
        _selectedPlayerPiece.SetHighlight(true, true);
    }

    /// <summary>
    /// Handles all input.
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
            HoldInput(LevelManager.Instance.GetInputState());
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
                TapInput(LevelManager.Instance.GetInputState());
            }
            if (_keyHoldTimer >= 0f)
            {
                ReleaseInput(LevelManager.Instance.GetInputState());
            }
        }
            
        _keyTapTimer = keyTapThreshold;
        _keyHoldTimer = keyHoldThreshold;
        _isKeyDown = false;
    }

    /// <summary>
    /// Handles input for when the input key is tapped and not held.
    /// </summary>
    /// <param name="inputState">The current input state.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void TapInput(InputState inputState)
    {
        switch (inputState)
        {
            case InputState.SelectPiece:
                SelectPiece();
                break;
            case InputState.SelectTarget:
                SelectTarget();
                break;
            case InputState.Attack:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Handles input for when the input key is held.
    /// </summary>
    /// <param name="inputState">The current input state.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void HoldInput(InputState inputState)
    {
        switch (inputState)
        {
            case InputState.SelectPiece:
                ConfirmPiece();
                break;
            case InputState.SelectTarget:
                ConfirmTarget();
                break;
            case InputState.Attack:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Handles input for when the input key is released.
    /// </summary>
    /// <param name="inputState">The current input state.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ReleaseInput(InputState inputState)
    {
        switch (inputState)
        {
            case InputState.SelectPiece:
                PerformSelectedPieceReleaseInput();
                break;
            case InputState.SelectTarget:
                PerformTargetPieceReleaseInput();
                break;
            case InputState.Attack:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Deactivates the current selected piece and selects the next piece in the list.
    /// </summary>
    private void SelectPiece()
    {
        _selectedPlayerPiece.SetHighlight(false, true);
        _selectedPlayerPieceIndex = (_selectedPlayerPieceIndex + 1) % _pieces.Count;
        _selectedPlayerPiece = _pieces[_selectedPlayerPieceIndex];
        _selectedPlayerPiece.SetHighlight(true, true);
    }

    /// <summary>
    /// Starts the progress bar animation for the selected piece, only if the coroutine has not already begun.
    /// </summary>
    private void ConfirmPiece()
    {
        if (_coroutineStarted) return;
           
        _coroutineStarted = true;
        _currentCoroutine = _selectedPlayerPiece.StartProgressBar(keyHoldThreshold, NumSteps, true);
    }

    /// <summary>
    /// Deactivates the current selected target piece and selects the next target piece in the list.
    /// </summary>
    private void SelectTarget()
    {
        if (_selectedTargetPiece == _selectedPlayerPiece)
        {
            _selectedTargetPiece.SetHighlight(true, true);
        }
        else
        {
            _selectedTargetPiece.SetHighlight(false, false);
        }

        _selectedTargetPieceIndex = (_selectedTargetPieceIndex + 1) % _targetPieces.Count;
        _selectedTargetPiece = _targetPieces[_selectedTargetPieceIndex];
        _selectedTargetPiece.SetHighlight(true, false);

        // Reparent the pattern overlay
        if (PatternOverlay != null)
        {
            PatternOverlay.transform.SetParent(_selectedTargetPiece.transform, false);
        }
    }

    /// <summary>
    /// Starts the progress bar animation for the selected target piece, only if the coroutine has not already begun.
    /// TODO: Implement
    /// </summary>
    private void ConfirmTarget()
    {
        if (_coroutineStarted) return;
        
        _coroutineStarted = true;
        _currentCoroutine = _selectedTargetPiece.StartProgressBar(keyHoldThreshold, NumSteps, false);
    }

    /// <summary>
    /// When the progress bar animation has been completed, confirm the selected piece and get the list of possible
    /// target pieces for the player to choose from.
    /// </summary>
    /// <param name="selectedPiece">The player's selected piece to attack with.</param>
    private void ConfirmSelectedPiece(ChessPiece selectedPiece)
    {
        _coroutineStarted = false;
        _selectedPlayerPiece.SetHighlight(false, true);
        _selectedPlayerPiece = selectedPiece;
        _selectedPlayerPieceIndex = _pieces.IndexOf(selectedPiece);
        _selectedPlayerPiece.SetHighlight(true, true);
        _targetPieces = Board.Instance.GetOpponentPieces().Append(_selectedPlayerPiece).ToList();
        _selectedTargetPiece = _targetPieces[_selectedTargetPieceIndex];
        _selectedTargetPiece.SetHighlight(true, false);
        // Setup the pattern
        PatternOverlay =  Instantiate(PatternOverlayPrefab, _selectedTargetPiece.transform, false);
        PatternRenderer patternRenderer = PatternOverlay.GetComponent<PatternRenderer>();
        patternRenderer.ControllingPiece = _selectedPlayerPiece;
        patternRenderer.DrawPattern();
    }

    /// <summary>
    /// Launches the player's selected piece's attack at the specified target piece to attack.
    /// TODO: finish implementing.
    /// </summary>
    /// <param name="pieceToAttack">The piece to attack.</param>
    private void Attack(ChessPiece pieceToAttack)
    {
        _coroutineStarted = false;
        _selectedPlayerPiece.SetHighlight(false, true);
        _selectedTargetPiece.SetHighlight(false, false);
        Destroy(PatternOverlay);
        // todo: spawn attack at pieceToAttack's position
        Debug.Log($"Attacking {pieceToAttack.name} at {pieceToAttack.transform.position}");
        GameEvent.CompleteTurn();
    }

    /// <summary>
    /// Interrupts and stops the selected piece's progress bar animation coroutine if it is running.
    /// </summary>
    private void PerformSelectedPieceReleaseInput()
    {
        if (!_coroutineStarted) return;
        _selectedPlayerPiece.StopProgressBar(_currentCoroutine);
        _coroutineStarted = false;
    }
    
    /// <summary>
    /// Interrupts and stops the target piece's progress bar animation coroutine if it is running.
    /// </summary>
    private void PerformTargetPieceReleaseInput()
    {
        if (!_coroutineStarted) return;
        _selectedTargetPiece.StopProgressBar(_currentCoroutine);
        _coroutineStarted = false;
    }
    
    /// <summary>
    /// Unsubscribes from game events.
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnConfirmSelectedPiece -= ConfirmSelectedPiece;
        GameEvent.OnAttackTarget -= Attack;
    }
}