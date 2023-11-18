using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance = null;
    
    private List<ChessPiece> _playerPieces = new ();
    private List<ChessPiece> _opponentPieces = new ();

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
    
    public List<ChessPiece> GetPlayerPieces() => _playerPieces;
    
    public List<ChessPiece> GetOpponentPieces() => _opponentPieces;
}