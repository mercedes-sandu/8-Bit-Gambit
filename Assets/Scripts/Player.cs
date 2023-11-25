using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Serialization;
using InputState = LevelManager.InputState;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode inputKey = KeyCode.Space;
    
    [SerializeField] private float keyTapThreshold = 0.5f;
    [SerializeField] private float keyHoldThreshold = 2f;

    public GameObject PatternOverlayPrefab;
    private GameObject PatternOverlay;
    
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
        _keyTapTimer = keyTapThreshold;
        _keyHoldTimer = keyHoldThreshold;
        
        GameEvent.OnConfirmSelectedPiece += ConfirmSelectedPiece;
        GameEvent.OnAttackTarget += Attack;
        GameEvent.OnPieceDie += RefreshPieces;
    }
    
    /// <summary>
    /// Get all the player's pieces and select the first one.
    /// </summary>
    private void Start()
    {
        _pieces = Board.Instance.GetPlayerPieces().ToList();
        PlayerTurn();
    }

    /// <summary>
    /// Handles all input.
    /// </summary>
    private void Update()
    {
        if (!LevelManager.Instance.GetIsPlayerTurn() ||
            LevelManager.Instance.GetInputState() == InputState.CanvasEnabled) return;
        
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
        GameEvent.PlayerTogglePiece(_selectedPlayerPiece, InputState.SelectPiece);
    }

    /// <summary>
    /// Starts the progress bar animation for the selected piece, only if the coroutine has not already begun.
    /// </summary>
    private void ConfirmPiece()
    {
        if (_coroutineStarted) return;
           
        _coroutineStarted = true;
        _currentCoroutine = _selectedPlayerPiece.StartProgressBar(keyHoldThreshold, true);
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
        if (PatternOverlay) PatternOverlay.transform.SetParent(_selectedTargetPiece.transform, false);
        
        GameEvent.PlayerTogglePiece(_selectedTargetPiece, InputState.SelectTarget);
    }

    /// <summary>
    /// Starts the progress bar animation for the selected target piece, only if the coroutine has not already begun.
    /// </summary>
    private void ConfirmTarget()
    {
        if (_coroutineStarted) return;
        
        _coroutineStarted = true;
        _currentCoroutine = _selectedTargetPiece.StartProgressBar(keyHoldThreshold, false);
    }

    /// <summary>
    /// When the progress bar animation has been completed, confirm the selected piece and get the list of possible
    /// target pieces for the player to choose from.
    /// </summary>
    /// <param name="selectedPiece">The player's selected piece to attack with.</param>
    /// <param name="isPlayer">True if it is the player's turn, false otherwise.</param>
    private void ConfirmSelectedPiece(ChessPiece selectedPiece, bool isPlayer)
    {
        if (!isPlayer) return;
        
        _coroutineStarted = false;
        _selectedPlayerPiece.SetHighlight(false, true);
        _selectedPlayerPiece = selectedPiece;
        _selectedPlayerPieceIndex = _pieces.IndexOf(selectedPiece);
        _selectedPlayerPiece.SetHighlight(true, true);
        _targetPieces = Board.Instance.GetOpponentPieces().Append(_selectedPlayerPiece).ToList();
        _selectedTargetPiece = _targetPieces[_selectedTargetPieceIndex];
        _selectedTargetPiece.SetHighlight(true, false);
        GameEvent.PlayerTogglePiece(_selectedTargetPiece, InputState.SelectTarget);
        
        // Setup the pattern
        PatternOverlay =  Instantiate(PatternOverlayPrefab, _selectedTargetPiece.transform, false);
        var patternRenderer = PatternOverlay.GetComponent<PatternRenderer>();
        patternRenderer.ControllingPiece = _selectedPlayerPiece;
        patternRenderer.DrawPattern();
    }

    /// <summary>
    /// Launches the player's selected piece's attack at the specified target piece to attack.
    /// </summary>
    /// <param name="pieceToAttack">The piece to attack.</param>
    /// <param name="isPlayer">True if it is the player's turn, false otherwise.</param>
    private void Attack(ChessPiece pieceToAttack, bool isPlayer)
    {
        if (!isPlayer) return;
        
        _coroutineStarted = false;
        _selectedPlayerPiece.SetHighlight(false, true);
        _selectedTargetPiece.SetHighlight(false, false);
        
        var allPieces = Board.Instance.GetAllPieces().ToList();
        foreach (var piece in allPieces)
        {
            piece.CheckIfTargeted();
            piece.Explode();
        }
        PatternOverlay.GetComponent<PatternRenderer>().TriggerAnimations();
        GameEvent.ShakeCamera();
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
    /// 
    /// </summary>
    public void PlayerTurn()
    {
        if (!LevelManager.Instance.GetIsPlayerTurn()) return;

        _isKeyDown = false;
        _keyHoldTimer = keyHoldThreshold;
        _keyTapTimer = keyTapThreshold;
        _selectedPlayerPieceIndex = 0;
        _selectedTargetPieceIndex = 0;
        _selectedPlayerPiece = _pieces[_selectedPlayerPieceIndex];
        _selectedPlayerPiece.SetHighlight(true, true);
        GameEvent.PlayerTogglePiece(_selectedPlayerPiece, InputState.SelectPiece);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="isPlayer"></param>
    private void RefreshPieces(ChessPiece piece, bool isPlayer)
    {
        if (!isPlayer) return;
        
        _pieces.Clear();
        _pieces = Board.Instance.GetPlayerPieces().ToList();
    }
    
    /// <summary>
    /// Unsubscribes from game events.
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnConfirmSelectedPiece -= ConfirmSelectedPiece;
        GameEvent.OnAttackTarget -= Attack;
        GameEvent.OnPieceDie -= RefreshPieces;
    }
}