using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public static Board Instance = null;
    
    public const int NumEdgesInTile = 4;

    private Tilemap _boardTilemap;
    
    private List<ChessPiece> _playerPieces = new ();
    private List<ChessPiece> _opponentPieces = new ();
    private List<ChessPiece> _allPieces = new ();

    // todo: i'm not sure if i'll actually use these or not tbh
    private List<ChessPiece> _playerCapturedPieces = new();
    private List<ChessPiece> _opponentCapturedPieces = new();
    
    private Dictionary<Vector3Int, string> _boardTileNames = new();
    
    private static readonly List<string> BoardTileLetters = new()
    {
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V",
        "W", "X", "Y", "Z"
    };

    /// <summary>
    /// Establishes the singleton and finds all chess pieces on the board.
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

        _boardTilemap = transform.GetChild(0).GetComponent<Tilemap>();
        SetBoardTileNames();
        FindChessPieces();
    }

    /// <summary>
    /// Assigns all chess pieces on the board to either player pieces or opponent pieces.
    /// </summary>
    private void FindChessPieces()
    {
        var chessPieces = 
            FindObjectsOfType<ChessPiece>().OrderBy(piece => piece.transform.position.x);
        
        foreach (var chessPiece in chessPieces)
        {
            if (chessPiece.IsPlayerPiece())
            {
                _playerPieces.Add(chessPiece);
            }
            else
            {
                _opponentPieces.Add(chessPiece);
            }
            _allPieces.Add(chessPiece);
        }
    }
    
    /// <summary>
    /// Returns the list of player pieces on the board.
    /// </summary>
    /// <returns>The player's pieces as a list.</returns>
    public List<ChessPiece> GetPlayerPieces() => _playerPieces;

    /// <summary>
    /// Removes the specified piece from the list of player pieces.
    /// </summary>
    /// <param name="piece">The player piece to remove.</param>
    public void RemovePlayerPiece(ChessPiece piece)
    {
        _playerPieces.Remove(piece);
        _allPieces.Remove(piece);
        CapturePiece(piece, true);
        GameEvent.PieceDie(piece, true);
    }

    /// <summary>
    /// Returns the list of opponent pieces on the board.
    /// </summary>
    /// <returns>The opponent's pieces as a list.</returns>
    public List<ChessPiece> GetOpponentPieces() => _opponentPieces;
    
    /// <summary>
    /// Removes the specified piece from the list of opponent pieces and returns the modified list.
    /// </summary>
    /// <param name="piece">The opponent piece to remove.</param>
    public void RemoveOpponentPiece(ChessPiece piece)
    {
        _opponentPieces.Remove(piece);
        _allPieces.Remove(piece);
        CapturePiece(piece, false);
        GameEvent.PieceDie(piece, false);
    }

    /// <summary>
    /// Called when a piece is captured/removed. Adds the piece to the appropriate list of captured pieces and calls
    /// for a UI update.
    /// </summary>
    /// <param name="piece">The piece which was captured.</param>
    /// <param name="playerCaptured">True if the player's piece was captured, false if the opponent's piece was
    /// captured.</param>
    private void CapturePiece(ChessPiece piece, bool playerCaptured)
    {
        if (playerCaptured)
        {
            _playerCapturedPieces.Add(piece);
        }
        else
        {
            _opponentCapturedPieces.Add(piece);
        }
        
        LevelManager.Instance.UpdateCapturedPieces(piece, playerCaptured);
    }

    /// <summary>
    /// Gets all of the pieces on the board.
    /// </summary>
    /// <returns>The list of all chess pieces on the board.</returns>
    public List<ChessPiece> GetAllPieces() => _allPieces;

    /// <summary>
    /// 
    /// </summary>
    private void SetBoardTileNames()
    {
        var bounds = _boardTilemap.cellBounds;
        
        for (var row = bounds.yMax; row > bounds.yMin; row--)
        {
            for (var column = bounds.xMin; column < bounds.yMax; column++)
            {
                var tilePosition = new Vector3Int(column, row, 0);

                if (!_boardTilemap.GetTile(tilePosition)) continue;
                
                var tileName = BoardTileLetters[row - bounds.yMin] + (column + 1 - bounds.xMin);
                _boardTileNames.Add(tilePosition, tileName);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
    public string GetBoardTileName(Vector3 tilePosition) => _boardTileNames[_boardTilemap.WorldToCell(tilePosition)];
}