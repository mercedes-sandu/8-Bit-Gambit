public static class GameEvent
{
    public delegate void ConfirmSelectedPieceHandler(ChessPiece selectedPiece);
    public delegate void AttackTargetHandler(ChessPiece targetPiece);
    public delegate void TurnHandler();
    
    public static event ConfirmSelectedPieceHandler OnConfirmSelectedPiece;
    public static event AttackTargetHandler OnAttackTarget;
    public static event TurnHandler OnTurnComplete;
    
    public static void ConfirmSelectedPiece(ChessPiece selectedPiece) => OnConfirmSelectedPiece?.Invoke(selectedPiece);
    public static void AttackTarget(ChessPiece targetPiece) => OnAttackTarget?.Invoke(targetPiece);
    public static void CompleteTurn() => OnTurnComplete?.Invoke();
}