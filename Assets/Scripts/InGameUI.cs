using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    
    [SerializeField] private Canvas levelWonCanvas;
    [SerializeField] private Canvas levelLostCanvas;
    [SerializeField] private Canvas levelDrawCanvas;

    private bool _isPlayerTurn = true;
    
    /// <summary>
    /// Subscribes to the OnTurnComplete event.
    /// </summary>
    private void Start()
    {
        GameEvent.OnTurnComplete += UpdateTurnText;
    }
    
    /// <summary>
    /// Updates the turn text to reflect whose turn it is.
    /// </summary>
    private void UpdateTurnText()
    {
        _isPlayerTurn = !_isPlayerTurn;
        turnText.text = _isPlayerTurn ? "Player's Turn" : "Opponent's Turn";
    }
    
    /// <summary>
    /// When the player won the level, enable the level won canvas and set the input state to canvas enabled.
    /// </summary>
    public void LevelWon()
    {
        LevelManager.Instance.SetCanvasEnabled(true);
        levelWonCanvas.enabled = true;
    }
    
    /// <summary>
    /// When the player lost the level, enable the level lost canvas and set the input state to canvas enabled.
    /// </summary>
    public void LevelLost()
    {
        LevelManager.Instance.SetCanvasEnabled(true);
        levelLostCanvas.enabled = true;
    }
    
    /// <summary>
    /// When the level was a draw, enable the level draw canvas and set the input state to canvas enabled.
    /// </summary>
    public void LevelDraw()
    {
        LevelManager.Instance.SetCanvasEnabled(true);
        levelDrawCanvas.enabled = true;
    }

    /// <summary>
    /// Called by buttons in levelWonCanvas, levelLostCanvas, and levelDrawCanvas to load the appropriate next level
    /// or reload the current level.
    /// </summary>
    public void LoadLevel()
    {
        GameMaster.LoadLevel();
    }

    /// <summary>
    /// When a piece has been captured, it is added to the left of the board next to either the player or the opponent.
    /// </summary>
    /// <param name="piece">The piece which was captured.</param>
    /// <param name="playerCaptured">True if a player piece was captured, false if an opponent piece was captured.
    /// </param>
    public void AddCapturedPiece(ChessPiece piece, bool playerCaptured)
    {
        // todo: add the captured piece to the right ui container
    }
}