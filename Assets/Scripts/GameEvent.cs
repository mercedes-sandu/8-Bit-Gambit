public static class GameEvent
{
    public delegate void SelectPieceHandler(ChessPiece selectedPiece);
    public delegate void AttackTargetHandler(ChessPiece targetPiece);
    
    public static event SelectPieceHandler OnSelectPiece;
    public static event AttackTargetHandler OnAttackTarget;
    
    public static void SelectPiece(ChessPiece selectedPiece) => OnSelectPiece?.Invoke(selectedPiece);
    public static void AttackTarget(ChessPiece targetPiece) => OnAttackTarget?.Invoke(targetPiece);
}