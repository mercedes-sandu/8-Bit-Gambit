﻿public static class GameEvent
{
    /// <summary>
    /// Handles the event when a player has completed the progress bar animation when selecting their own piece.
    /// </summary>
    public delegate void ConfirmSelectedPieceHandler(ChessPiece selectedPiece);
    
    /// <summary>
    /// Handles the event when a player has completed the progress bar animation when selecting the piece to attack.
    /// </summary>
    public delegate void AttackTargetHandler(ChessPiece targetPiece);
    
    /// <summary>
    /// Handles the event when a turn has been completed.
    /// </summary>
    public delegate void TurnHandler();
    
    /// <summary>
    /// The event listener for when a player has completed the progress bar animation when selecting their own piece.
    /// </summary>
    public static event ConfirmSelectedPieceHandler OnConfirmSelectedPiece;
    
    /// <summary>
    /// The event listener for when a player has completed the progress bar animation when selecting the piece to
    /// attack.
    /// </summary>
    public static event AttackTargetHandler OnAttackTarget;
    
    /// <summary>
    /// The event listener for when a turn has been completed.
    /// </summary>
    public static event TurnHandler OnTurnComplete;
    
    /// <summary>
    /// The event invoker for when a player has completed the progress bar animation when selecting their own piece.
    /// </summary>
    /// <param name="selectedPiece">The player's piece the player has selected.</param>
    public static void ConfirmSelectedPiece(ChessPiece selectedPiece) => OnConfirmSelectedPiece?.Invoke(selectedPiece);
    
    /// <summary>
    /// The event invoker for when a player has completed the progress bar animation when selecting the piece to attack.
    /// </summary>
    /// <param name="targetPiece">The player's selected target piece.</param>
    public static void AttackTarget(ChessPiece targetPiece) => OnAttackTarget?.Invoke(targetPiece);
    
    /// <summary>
    /// The event invoker for when a turn has been completed.
    /// </summary>
    public static void CompleteTurn() => OnTurnComplete?.Invoke();
}