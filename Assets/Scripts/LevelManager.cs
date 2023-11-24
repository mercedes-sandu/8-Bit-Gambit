using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;

    [SerializeField] private InGameUI inGameUI;
    
    private InputState _inputState = InputState.SelectPiece;
    
    private bool _isPlayerTurn = true;

    private Sprite[] _greenProgressBarSprites;
    private Sprite[] _redProgressBarSprites;
    
    /// <summary>
    /// The different input states the player and opponent can be in.
    /// </summary>
    public enum InputState
    {
        SelectPiece, SelectTarget, Attack, CanvasEnabled
    }

    /// <summary>
    /// 
    /// </summary>
    public enum EndState
    {
        PlayerWon, PlayerLost, Draw
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
        
        _greenProgressBarSprites = Resources.LoadAll<Sprite>("Sprites/Piece Progress Bars/Green Progress Bar");
        _redProgressBarSprites = Resources.LoadAll<Sprite>("Sprites/Piece Progress Bars/Red Progress Bar");
        
        GameEvent.OnConfirmSelectedPiece += ConfirmSelectedPiece;
        GameEvent.OnAttackTarget += Attack;
        GameEvent.OnTurnComplete += CompleteTurn;
    }

    /// <summary>
    /// When an attacking piece has been selected, change the input state to selecting a target piece.
    /// </summary>
    /// <param name="selectedPiece">The piece selected.</param>
    /// <param name="isPlayer">True if it is the player's turn, false otherwise.</param>
    private void ConfirmSelectedPiece(ChessPiece selectedPiece, bool isPlayer)
    {
        _inputState = InputState.SelectTarget;
    }

    /// <summary>
    /// When a target piece has been selected, change the input state to attacking.
    /// </summary>
    /// <param name="targetPiece">The target piece selected.</param>
    /// <param name="isPlayer">True if it is the player's turn, false otherwise.</param>
    private void Attack(ChessPiece targetPiece, bool isPlayer)
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
        inGameUI.FadeDetailsPanel(_isPlayerTurn);
    }
    
    /// <summary>
    /// Returns the current input state.
    /// </summary>
    /// <returns>The current input state.</returns>
    public InputState GetInputState() => _inputState;

    /// <summary>
    /// Gets the progress bar sprites.
    /// </summary>
    /// <returns>A pair (green, red) of the progress bar sprites.</returns>
    public (Sprite[], Sprite[]) GetProgressBarSprites() => (_greenProgressBarSprites, _redProgressBarSprites);
    
    /// <summary>
    /// Gets whether or not it is the player's turn.
    /// </summary>
    /// <returns>True if it is the player's turn, false if it is the opponent's turn.</returns>
    public bool GetIsPlayerTurn() => _isPlayerTurn;
    
    /// <summary>
    /// Sets the input state to canvas enabled or selecting a piece, depending on whether level complete canvases are
    /// enabled
    /// </summary>
    /// <param name="canvasEnabled">True if a canvas is enabled, false otherwise.</param>
    public void SetCanvasEnabled(bool canvasEnabled)
    {
        _inputState = canvasEnabled ? InputState.CanvasEnabled : InputState.SelectPiece;
    }

    /// <summary>
    /// Checks if the level is over and calls the appropriate event.
    /// </summary>
    public void CheckForLevelOver()
    {
        var numPlayerPieces = Board.Instance.GetPlayerPieces().Count;
        var numOpponentPieces = Board.Instance.GetOpponentPieces().Count;
        
        switch (numPlayerPieces)
        {
            case 0 when numOpponentPieces == 0:
                GameEvent.LevelOver(EndState.Draw);
                break;
            case 0:
                GameEvent.LevelOver(EndState.PlayerLost);
                break;
            default:
            {
                if (numOpponentPieces == 0)
                {
                    GameEvent.LevelOver(EndState.PlayerWon);
                }

                break;
            }
        }
    }
    
    /// <summary>
    /// Called by GameMaster when the player won a level to display the levelWonCanvas.
    /// </summary>
    public void LevelWon()
    {
        inGameUI.LevelWon();
    }
    
    /// <summary>
    /// Called by GameMaster when the player lost a level to display the levelLostCanvas.
    /// </summary>
    public void LevelLost()
    {
        inGameUI.LevelLost();
    }
    
    /// <summary>
    /// Called by GameMaster when the level was a draw to display the levelDrawCanvas.
    /// </summary>
    public void LevelDraw()
    {
        inGameUI.LevelDraw();
    }

    /// <summary>
    /// Called when a piece is captured to update the UI accordingly.
    /// </summary>
    /// <param name="piece">The piece that was captured.</param>
    /// <param name="playerCaptured">True if a player piece was captured, false if an opponent piece was captured.
    /// </param>
    public void UpdateCapturedPieces(ChessPiece piece, bool playerCaptured)
    {
        inGameUI.AddCapturedPiece(piece, playerCaptured);
    }
    
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