using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;
    
    private InputState _inputState = InputState.SelectPiece;
    
    private bool _isPlayerTurn = true;
    
    /// <summary>
    /// The different input states the player and opponent can be in.
    /// </summary>
    public enum InputState
    {
        SelectPiece, SelectTarget, Attack
    }
    
    /// <summary>
    /// Establishes the singleton and subscribes to game events.
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
    /// When an attacking piece has been selected, change the input state to selecting a target piece.
    /// </summary>
    /// <param name="selectedPiece">The piece selected.</param>
    private void ConfirmSelectedPiece(ChessPiece selectedPiece)
    {
        _inputState = InputState.SelectTarget;
    }

    /// <summary>
    /// When a target piece has been selected, change the input state to attacking.
    /// </summary>
    /// <param name="targetPiece">The target piece selected.</param>
    private void Attack(ChessPiece targetPiece)
    {
        _inputState = InputState.Attack;
    }

    /// <summary>
    /// When a turn has been completed, change the input state to selecting a piece and change the turn.
    /// </summary>
    private void CompleteTurn()
    {
        _isPlayerTurn = !_isPlayerTurn;
        _inputState = InputState.SelectPiece;
    }
    
    /// <summary>
    /// Returns the current input state.
    /// </summary>
    /// <returns>The current input state.</returns>
    public InputState GetInputState() => _inputState;
    
    /// <summary>
    /// Unsubscribes from game events.
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnConfirmSelectedPiece -= ConfirmSelectedPiece;
        GameEvent.OnAttackTarget -= Attack;
        GameEvent.OnTurnComplete -= CompleteTurn;
    }
}