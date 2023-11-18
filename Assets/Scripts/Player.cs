using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InputState = LevelManager.InputState;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode inputKey = KeyCode.Space;
    
    [SerializeField] private float timeToHold = 3f;

    private const int NumSteps = 5;

    private List<ChessPiece> _pieces = new();
    private ChessPiece _selectedPlayerPiece;
    private int _selectedPlayerPieceIndex = 0;

    private List<ChessPiece> _targetPieces = new();
    private ChessPiece _selectedTargetPiece;
    private int _selectedTargetPieceIndex = 0;

    private bool _firstKeyPress = false;
    
    private float _pressStartTime;
    private bool _wasKeyPressed = false;
    private const float HoldTimeThreshold = 0.5f;
    private bool _holdingInputKey = false;

    private Coroutine _currentCoroutine;
    private bool _coroutineStarted = false;

    private void Awake()
    {
        GameEvent.OnConfirmSelectedPiece += ConfirmSelectedPiece;
        GameEvent.OnAttackTarget += Attack;
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
        if (!_wasKeyPressed && !_holdingInputKey && Input.GetKeyDown(inputKey))
        {
            TapInput(LevelManager.Instance.GetInputState());
            _pressStartTime = Time.time;
            _wasKeyPressed = true;
        }
        
        if (Input.GetKey(inputKey) && Time.time - _pressStartTime > HoldTimeThreshold)
        {
            _holdingInputKey = true;
            HoldInput(LevelManager.Instance.GetInputState());
        }

        if (!Input.GetKeyUp(inputKey)) return;
        if (!_holdingInputKey) return;
        _wasKeyPressed = false;
        _holdingInputKey = false;
        ReleaseInput(LevelManager.Instance.GetInputState());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputState"></param>
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
    /// 
    /// </summary>
    /// <param name="inputState"></param>
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

    private void ReleaseInput(InputState inputState)
    {
        switch (inputState)
        {
            case InputState.SelectPiece:
                PerformReleaseInput();
                break;
            case InputState.SelectTarget:
                PerformReleaseInput();
                break;
            case InputState.Attack:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void SelectPiece()
    {
        _selectedPlayerPiece.SetHighlight(false, true);
        _selectedPlayerPieceIndex = (_selectedPlayerPieceIndex + 1) % _pieces.Count;
        _selectedPlayerPiece = _pieces[_selectedPlayerPieceIndex];
        _selectedPlayerPiece.SetHighlight(true, true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ConfirmPiece()
    {
        if (_coroutineStarted) return;
           
        _coroutineStarted = true;
        _currentCoroutine = _selectedPlayerPiece.StartProgressBar(timeToHold, NumSteps);
    }

    /// <summary>
    /// 
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
    }

    /// <summary>
    /// 
    /// </summary>
    private void ConfirmTarget()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedPiece"></param>
    private void ConfirmSelectedPiece(ChessPiece selectedPiece)
    {
        _holdingInputKey = false;
        _coroutineStarted = false;
        _selectedPlayerPiece.SetHighlight(false, true);
        _selectedPlayerPiece = selectedPiece;
        _selectedPlayerPieceIndex = _pieces.IndexOf(selectedPiece);
        _selectedPlayerPiece.SetHighlight(true, true);
        _targetPieces = Board.Instance.GetOpponentPieces().Append(_selectedPlayerPiece).ToList();
        _selectedTargetPiece = _targetPieces[_selectedTargetPieceIndex];
        _selectedTargetPiece.SetHighlight(true, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pieceToAttack"></param>
    private void Attack(ChessPiece pieceToAttack)
    {
        _coroutineStarted = false;
        // todo: spawn attack at pieceToAttack's position
        Debug.Log($"Attacking {pieceToAttack.name} at {pieceToAttack.transform.position}");
        GameEvent.CompleteTurn();
    }

    private void PerformReleaseInput()
    {
        if (!_coroutineStarted) return;
        _selectedPlayerPiece.StopProgressBar(_currentCoroutine);
        _coroutineStarted = false;
        _holdingInputKey = false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnConfirmSelectedPiece -= ConfirmSelectedPiece;
        GameEvent.OnAttackTarget -= Attack;
    }
}