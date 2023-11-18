using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance = null;
    
    private List<ChessPiece> _playerPieces = new ();
    private List<ChessPiece> _opponentPieces = new ();
    
    private const int NumEdgesInTile = 4;

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
        }
    }
    
    /// <summary>
    /// Returns the list of player pieces on the board.
    /// </summary>
    /// <returns>The player's pieces as a list.</returns>
    public List<ChessPiece> GetPlayerPieces() => _playerPieces;

    /// <summary>
    /// Removes the specified piece from the list of player pieces and returns the modified list.
    /// </summary>
    /// <param name="piece">The player piece to remove.</param>
    /// <returns>The modified player pieces list with the specified piece removed.</returns>
    public List<ChessPiece> RemovePlayerPiece(ChessPiece piece)
    {
        _playerPieces.Remove(piece);
        return _playerPieces;
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
    /// <returns>The modified opponent pieces list with the specified piece removed.</returns>
    public List<ChessPiece> RemoveOpponentPiece(ChessPiece piece)
    {
        _opponentPieces.Remove(piece);
        return _opponentPieces;
    }
}