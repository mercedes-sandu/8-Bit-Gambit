using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;
    
    private InputState _inputState = InputState.SelectPiece;
    
    private bool _isPlayerTurn = true;
    
    /// <summary>
    /// 
    /// </summary>
    public enum InputState
    {
        SelectPiece, SelectTarget, Attack
    }
    
    /// <summary>
    /// 
    /// </summary>
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
        
        GameEvent.OnConfirmSelectedPiece += ConfirmSelectedPiece;
        GameEvent.OnAttackTarget += Attack;
        GameEvent.OnTurnComplete += CompleteTurn;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedPiece"></param>
    private void ConfirmSelectedPiece(ChessPiece selectedPiece)
    {
        _inputState = InputState.SelectTarget;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetPiece"></param>
    private void Attack(ChessPiece targetPiece)
    {
        _inputState = InputState.Attack;
    }

    /// <summary>
    /// 
    /// </summary>
    private void CompleteTurn()
    {
        _isPlayerTurn = !_isPlayerTurn;
        _inputState = InputState.SelectPiece;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public InputState GetInputState() => _inputState;
}