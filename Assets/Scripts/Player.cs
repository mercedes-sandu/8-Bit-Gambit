using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode inputKey = KeyCode.Space;
    
    [SerializeField] private float timeToHold = 3f;

    private List<ChessPiece> _pieces = new();
    private ChessPiece _selectedPiece;
    private int _selectedPieceIndex = 0;
    
    private InputState _inputState = InputState.SelectPiece;

    /// <summary>
    /// Get all the player's pieces and select the first one.
    /// </summary>
    private void Start()
    {
        _pieces = Board.Instance.GetPlayerPieces();
        _selectedPiece = _pieces[_selectedPieceIndex];
        _selectedPiece.SetHighlight(true);
    }

    /// <summary>
    /// Handles all input.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(inputKey))
        {
            TapInput();
        }
        
        if (Input.GetKey(inputKey))
        {
            HoldInput();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void TapInput()
    {
        switch (_inputState)
        {
            case InputState.SelectPiece:
                SelectPiece();
                break;
            case InputState.ConfirmPiece:
                break;
            case InputState.SelectTarget:
                SelectTarget();
                break;
            case InputState.Attack:
                Attack();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
    private void SelectPiece()
    {
        _selectedPiece.SetHighlight(false);
        _selectedPieceIndex = (_selectedPieceIndex + 1) % _pieces.Count;
        _selectedPiece = _pieces[_selectedPieceIndex];
        _selectedPiece.SetHighlight(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ConfirmPiece()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void SelectTarget()
    {
        
    }

    /// <summary>
    /// Note: This is the same as ConfirmTarget. The player will have to hold down the input key and the attack will
    /// launch after a certain amount of time.
    /// </summary>
    private void Attack()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    private enum InputState
    {
        SelectPiece, ConfirmPiece, SelectTarget, Attack
    }
}