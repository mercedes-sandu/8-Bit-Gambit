using System;
using System.Collections.Generic;
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

    private ChessPiece _selectedTargetPiece;
    private int _selectedTargetPieceIndex = 0;

    private float _pressStartTime;
    private const float HoldTimeThreshold = 0.5f;

    private Coroutine _currentCoroutine;
    private bool _coroutineStarted = false;

    /// <summary>
    /// Get all the player's pieces and select the first one.
    /// </summary>
    private void Start()
    {
        _pieces = Board.Instance.GetPlayerPieces();
        _selectedPlayerPiece = _pieces[_selectedPlayerPieceIndex];
        _selectedPlayerPiece.SetHighlight(true);
    }

    /// <summary>
    /// Handles all input.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(inputKey))
        {
            TapInput(LevelManager.Instance.GetInputState());
            _pressStartTime = Time.time;
        }

        if (!Input.GetKey(inputKey)) return;
        if (Time.time - _pressStartTime > HoldTimeThreshold)
        {
            HoldInput(LevelManager.Instance.GetInputState());
        }
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
        switch (LevelManager.Instance.GetInputState())
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
    /// 
    /// </summary>
    private void SelectPiece()
    {
        _selectedPlayerPiece.SetHighlight(false);
        _selectedPlayerPieceIndex = (_selectedPlayerPieceIndex + 1) % _pieces.Count;
        _selectedPlayerPiece = _pieces[_selectedPlayerPieceIndex];
        _selectedPlayerPiece.SetHighlight(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ConfirmPiece()
    {
        if (_coroutineStarted)
        {
            _selectedPlayerPiece.StopProgressBar(_currentCoroutine);
            _coroutineStarted = false;
        }
                
        _currentCoroutine = _selectedPlayerPiece.StartProgressBar(timeToHold, NumSteps);
        _coroutineStarted = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SelectTarget()
    {
        
    }

    private void ConfirmTarget()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void Attack()
    {
        
    }
}