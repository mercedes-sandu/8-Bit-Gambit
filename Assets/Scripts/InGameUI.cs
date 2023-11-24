using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    
    [SerializeField] private Transform playerCapturedPiecesContainer;
    [SerializeField] private Transform opponentCapturedPiecesContainer;

    [SerializeField] private Tilemap backgroundTilemap;

    [SerializeField] private float tileFadeTime = 0.1f;
    [SerializeField] private float canvasFadeTime = 0.2f;
    
    [SerializeField] private Canvas levelWonCanvas;
    [SerializeField] private Canvas levelLostCanvas;
    [SerializeField] private Canvas levelDrawCanvas;

    private Canvas _canvas;
    
    private TilemapRenderer _backgroundTilemapRenderer;
    
    private Dictionary<string, GameObject> _pieceNameToUiPiece = new ();
    
    private bool _isPlayerTurn = true;
    
    /// <summary>
    /// Starts the overlay tilemap fade out coroutine, and subscribes to the OnTurnComplete event.
    /// </summary>
    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        _canvas.GetComponent<CanvasGroup>().alpha = 0;
        levelWonCanvas.enabled = false;
        levelWonCanvas.GetComponent<CanvasGroup>().alpha = 0;
        levelLostCanvas.enabled = false;
        levelLostCanvas.GetComponent<CanvasGroup>().alpha = 0;
        levelDrawCanvas.enabled = false;
        levelDrawCanvas.GetComponent<CanvasGroup>().alpha = 0;
        
        _backgroundTilemapRenderer = backgroundTilemap.GetComponent<TilemapRenderer>();
        
        InitializePieceNameToUiPieceDictionary();
        
        GameEvent.OnTurnComplete += UpdateTurnText;
        
        StartCoroutine(FadeAllTiles(false, _canvas));
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitializePieceNameToUiPieceDictionary()
    {
        var uiPieces = Resources.LoadAll<GameObject>("Prefabs/UI Pieces");
        foreach (var uiPiece in uiPieces)
        {
            _pieceNameToUiPiece.Add(uiPiece.name, uiPiece);
        }
    }
    
    /// <summary>
    /// Updates the turn text to reflect whose turn it is.
    /// </summary>
    private void UpdateTurnText()
    {
        _isPlayerTurn = !_isPlayerTurn;
        turnText.text = _isPlayerTurn ? "Player's Turn" : "Opponent's Turn";
    }
    
    #region Fading Coroutines

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fadeIn"></param>
    /// <param name="canvas"></param>
    /// <returns></returns>
    private IEnumerator FadeAllTiles(bool fadeIn, Canvas canvas)
    {
        _backgroundTilemapRenderer.enabled = true;
        SetOpacityAllTiles(!fadeIn);
        
        var bounds = backgroundTilemap.cellBounds;

        for (var x = bounds.xMin; x < bounds.xMax; x++)
        {
            var tilePosition = new Vector3Int(x, bounds.yMax - 1, 0);
            var tile = backgroundTilemap.GetTile(tilePosition);

            if (!tile) continue;

            var tilesToFade = GetDiagonalTiles(tilePosition, true);
            foreach (var tileToFade in tilesToFade)
            {
                StartCoroutine(FadeTile(tileToFade, fadeIn));
            }
            yield return new WaitForSeconds(tileFadeTime);
        }
        
        for (var y = bounds.yMax - 2; y >= bounds.yMin; y--)
        {
            var tilePosition = new Vector3Int(bounds.xMax - 1, y, 0);
            var tile = backgroundTilemap.GetTile(tilePosition);

            if (!tile) continue;

            var tilesToFade = GetDiagonalTiles(tilePosition, false);
            foreach (var tileToFade in tilesToFade)
            {
                StartCoroutine(FadeTile(tileToFade, fadeIn));
            }
            yield return new WaitForSeconds(tileFadeTime);
        }

        if (!fadeIn)
        {
            _backgroundTilemapRenderer.enabled = false;
            _canvas.enabled = true;
        }
        StartCoroutine(FadeCanvas(!fadeIn, canvas, canvas.GetComponent<CanvasGroup>()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="sweepingAxisX"></param>
    /// <returns></returns>
    private List<Vector3Int> GetDiagonalTiles(Vector3Int startPos, bool sweepingAxisX)
    {
        var tilesToFade = new List<Vector3Int>();
        var bounds = backgroundTilemap.cellBounds;
        var x = startPos.x;
        var y = startPos.y;

        if (sweepingAxisX)
        {
            while (x >= bounds.xMin && y >= bounds.yMin)
            {
                tilesToFade.Add(new Vector3Int(x, y, 0));
                x--;
                y--;
            } 
        }
        else
        {
            while (x >= bounds.xMin && y >= bounds.yMin)
            {
                tilesToFade.Add(new Vector3Int(x, y, 0));
                x--;
                y--;
            }
        }
        
        return tilesToFade;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="fadeIn"></param>
    /// <returns></returns>
    private IEnumerator FadeTile(Vector3Int tilePosition, bool fadeIn)
    {
        var originalColor = backgroundTilemap.GetColor(tilePosition);
        var targetAlpha = fadeIn ? 1f : 0f;
        var targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);

        var elapsedTime = 0f;

        while (elapsedTime < tileFadeTime)
        {
            var alpha = Mathf.Lerp(originalColor.a, targetAlpha, elapsedTime / tileFadeTime);
            var currentColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            backgroundTilemap.SetColor(tilePosition, currentColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        backgroundTilemap.SetColor(tilePosition, targetColor);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="opaque"></param>
    private void SetOpacityAllTiles(bool opaque)
    {
        var bounds = backgroundTilemap.cellBounds;

        for (var y = bounds.yMax - 1; y >= bounds.yMin; y--)
        {
            for (var x = bounds.xMin; x < bounds.xMax; x++)
            {
                var tilePosition = new Vector3Int(x, y, 0);
                var tile = backgroundTilemap.GetTile<Tile>(tilePosition);
                backgroundTilemap.SetTile(tilePosition, tile);
                backgroundTilemap.SetTileFlags(tilePosition, TileFlags.None);

                if (!tile) continue;
                backgroundTilemap.SetColor(tilePosition, 
                    opaque ? new Color(0.8f, 0.8f, 0.8f, 1) : new Color(0.8f, 0.8f, 0.8f, 0));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fadeIn"></param>
    /// <param name="canvas"></param>
    /// <param name="canvasGroup"></param>
    /// <returns></returns>
    private IEnumerator FadeCanvas(bool fadeIn, Canvas canvas, CanvasGroup canvasGroup)
    {
        var originalAlpha = canvasGroup.alpha;
        var targetAlpha = fadeIn ? 1f : 0f;

        var elapsedTime = 0f;

        while (elapsedTime < canvasFadeTime)
        {
            var alpha = Mathf.Lerp(originalAlpha, targetAlpha, elapsedTime / canvasFadeTime);
            canvasGroup.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
        if (!fadeIn) canvas.enabled = false;
        var levelStateCanvas = canvas.GetComponent<LevelStateCanvas>();
        if (levelStateCanvas) levelStateCanvas.EnableCanvasInput();
    }
    #endregion
    
    /// <summary>
    /// When the player won the level, enable the level won canvas and set the input state to canvas enabled.
    /// </summary>
    public void LevelWon()
    {
        StartCoroutine(FadeCanvas(false, _canvas, _canvas.GetComponent<CanvasGroup>()));
        LevelManager.Instance.SetCanvasEnabled(true);
        levelWonCanvas.enabled = true;
        StartCoroutine(FadeAllTiles(true, levelWonCanvas));
    }
    
    /// <summary>
    /// When the player lost the level, enable the level lost canvas and set the input state to canvas enabled.
    /// </summary>
    public void LevelLost()
    {
        StartCoroutine(FadeCanvas(false, _canvas, _canvas.GetComponent<CanvasGroup>()));
        LevelManager.Instance.SetCanvasEnabled(true);
        levelLostCanvas.enabled = true;
        StartCoroutine(FadeAllTiles(true, levelLostCanvas));
    }
    
    /// <summary>
    /// When the level was a draw, enable the level draw canvas and set the input state to canvas enabled.
    /// </summary>
    public void LevelDraw()
    {
        StartCoroutine(FadeCanvas(false, _canvas, _canvas.GetComponent<CanvasGroup>()));
        LevelManager.Instance.SetCanvasEnabled(true);
        levelDrawCanvas.enabled = true;
        StartCoroutine(FadeAllTiles(true, levelDrawCanvas));
    }

    /// <summary>
    /// When a piece has been captured, it is added to the left of the board next to either the player or the opponent.
    /// </summary>
    /// <param name="piece">The piece which was captured.</param>
    /// <param name="playerCaptured">True if a player piece was captured, false if an opponent piece was captured.
    /// </param>
    public void AddCapturedPiece(ChessPiece piece, bool playerCaptured)
    {
        // string formatting for dictionary purposes
        var pieceName = piece.name.Contains("(")
            ? piece.name[..piece.name.IndexOf("(", StringComparison.Ordinal)]
            : piece.name;

        Instantiate(_pieceNameToUiPiece[pieceName],
            playerCaptured ? opponentCapturedPiecesContainer : playerCapturedPiecesContainer);
    }
}