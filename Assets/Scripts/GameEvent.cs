public static class GameEvent
{
    /// <summary>
    /// Handles the event when a player has completed the progress bar animation when selecting their own piece.
    /// </summary>
    public delegate void ConfirmSelectedPieceHandler(ChessPiece selectedPiece, bool isPlayer);
    
    /// <summary>
    /// Handles the event when a player has completed the progress bar animation when selecting the piece to attack.
    /// </summary>
    public delegate void AttackTargetHandler(ChessPiece targetPiece, bool isPlayer);
    
    /// <summary>
    /// Handles the event when a turn has been completed.
    /// </summary>
    public delegate void TurnHandler();

    /// <summary>
    /// Handles the event in which the camera should shake.
    /// </summary>
    public delegate void CameraShakeHandler();

    /// <summary>
    /// Handles the event in which the level is over.
    /// </summary>
    public delegate void LevelOverHandler(LevelManager.EndState endState, string nextLevelName);
    
    /// <summary>
    /// Handles the event in which the player is toggling between their pieces during their turn.
    /// </summary>
    public delegate void PlayerTogglePieceHandler(ChessPiece selectedPiece, LevelManager.InputState inputState);

    /// <summary>
    /// Handles the event in which a piece has died.
    /// </summary>
    public delegate void PieceDieHandler(ChessPiece piece, bool isPlayer);
    
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
    /// The event listener for when the camera should shake.
    /// </summary>
    public static event CameraShakeHandler OnCameraShake;
    
    /// <summary>
    /// The event listener for when the level is over.
    /// </summary>
    public static event LevelOverHandler OnLevelOver;
    
    /// <summary>
    /// The event listener for when the player toggles between their pieces.
    /// </summary>
    public static event PlayerTogglePieceHandler OnPlayerTogglePiece;
    
    /// <summary>
    /// The event listener for when a piece has died.
    /// </summary>
    public static event PieceDieHandler OnPieceDie;

    /// <summary>
    /// The event invoker for when a player has completed the progress bar animation when selecting their own piece.
    /// </summary>
    /// <param name="selectedPiece">The player's piece the player has selected.</param>
    /// <param name="isPlayer">True if it is the player's turn, false otherwise.</param>
    public static void ConfirmSelectedPiece(ChessPiece selectedPiece, bool isPlayer) =>
        OnConfirmSelectedPiece?.Invoke(selectedPiece, isPlayer);

    /// <summary>
    /// The event invoker for when a player has completed the progress bar animation when selecting the piece to attack.
    /// </summary>
    /// <param name="targetPiece">The player's selected target piece.</param>
    /// <param name="isPlayer">True if it is the player's turn, false otherwise.</param>
    public static void AttackTarget(ChessPiece targetPiece, bool isPlayer) =>
        OnAttackTarget?.Invoke(targetPiece, isPlayer);
    
    /// <summary>
    /// The event invoker for when a turn has been completed.
    /// </summary>
    public static void CompleteTurn() => OnTurnComplete?.Invoke();
    
    /// <summary>
    /// The event invoker for when the camera should shake.
    /// </summary>
    public static void ShakeCamera() => OnCameraShake?.Invoke();

    /// <summary>
    /// The event invoker for when the level is over.
    /// </summary>
    /// <param name="endState">Whether the player won, lost, or there was a draw.</param>
    /// <param name="nextLevelName"></param>
    public static void LevelOver(LevelManager.EndState endState, string nextLevelName) =>
        OnLevelOver?.Invoke(endState, nextLevelName);

    /// <summary>
    /// The event invoker for when the player toggles between their pieces.
    /// </summary>
    /// <param name="selectedPiece">The current selected piece.</param>
    /// <param name="inputState">The current input state, either SelectPiece or SelectTarget.</param>
    public static void PlayerTogglePiece(ChessPiece selectedPiece, LevelManager.InputState inputState) =>
        OnPlayerTogglePiece?.Invoke(selectedPiece, inputState);
    
    /// <summary>
    /// The event invoker for when a piece has died.
    /// </summary>
    /// <param name="piece">The piece that has died.</param>
    /// <param name="isPlayer">True if a player piece has died, false if an opponent piece has died.</param>
    public static void PieceDie(ChessPiece piece, bool isPlayer) => OnPieceDie?.Invoke(piece, isPlayer);
}