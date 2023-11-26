using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI instructionsText;
    
    [SerializeField] private Transform playerCapturedPiecesContainer;
    [SerializeField] private Transform opponentCapturedPiecesContainer;

    [SerializeField] private Tilemap backgroundTilemap;

    [SerializeField] private float tileFadeTime = 0.1f;
    [SerializeField] private float canvasFadeTime = 0.2f;
    [SerializeField] private float pauseBeforeGameOverTime = 1f;
    
    [SerializeField] private Canvas levelWonCanvas;
    [SerializeField] private Canvas levelLostCanvas;
    [SerializeField] private Canvas levelDrawCanvas;
    
    // details panel UI
    [SerializeField] private Image detailsPanel;
    [SerializeField] private TextMeshProUGUI pieceNameText;
    [SerializeField] private TextMeshProUGUI durabilityText;
    [SerializeField] private Image[] durabilityCircleImages;
    [SerializeField] private Image attackPatternImage;
    
    [SerializeField] private Sprite circleFilledSprite;
    [SerializeField] private Sprite circleEmptySprite;

    private Canvas _canvas;
    private Animator _animator;
    private Image _attackTextContainerImage;

    private FadingText _instructionsTextFadingText;
    private WaveText _turnTextWaveText;
    private Coroutine _fadingTextCoroutine;
    private Coroutine _waveTextCoroutine;
    
    private TilemapRenderer _backgroundTilemapRenderer;
    
    private Dictionary<string, GameObject> _pieceNameToUiPiece = new ();
    private Dictionary<string, Sprite> _pieceNameToAttackPatternSprite = new ();
    
    private bool _isPlayerTurn = true;
    private bool _waitedForGameOver = false;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        InitializeDictionaries();
        
        GameEvent.OnTurnComplete += UpdateTurnText;
        GameEvent.OnPlayerTogglePiece += UpdateDetailsPanel;
        GameEvent.OnConfirmSelectedPiece += UpdateInstructionsText;
        GameEvent.OnAttackTarget += AttackUI;
    }
    
    /// <summary>
    /// 
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
        
        _instructionsTextFadingText = instructionsText.GetComponent<FadingText>();
        _turnTextWaveText = turnText.GetComponent<WaveText>();
        
        _fadingTextCoroutine = _instructionsTextFadingText.FadeTextRoutine();

        attackText.enabled = false;
        _attackTextContainerImage = attackText.transform.parent.GetComponent<Image>();
        _attackTextContainerImage.enabled = false;

        _animator = GetComponent<Animator>();
        
        _backgroundTilemapRenderer = backgroundTilemap.GetComponent<TilemapRenderer>();
        
        StartCoroutine(FadeAllTiles(false, _canvas, true));
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitializeDictionaries()
    {
        var uiPieces = Resources.LoadAll<GameObject>("Prefabs/UI Pieces");
        foreach (var uiPiece in uiPieces)
        {
            _pieceNameToUiPiece.Add(uiPiece.name, uiPiece);
        }
        
        var attackPatternSprites = Resources.LoadAll<Sprite>("Sprites/Attack Patterns");
        foreach (var attackPatternSprite in attackPatternSprites)
        {
            _pieceNameToAttackPatternSprite.Add(attackPatternSprite.name, attackPatternSprite);
        }
    }
    
    /// <summary>
    /// Updates the turn text to reflect whose turn it is. Also updates the instructions text.
    /// </summary>
    private void UpdateTurnText()
    {
        _isPlayerTurn = !_isPlayerTurn;
        turnText.text = _isPlayerTurn ? "Player's Turn" : "Opponent's Turn";
        if (_isPlayerTurn)
        {
            _instructionsTextFadingText.StopFadeTextCoroutine(_fadingTextCoroutine);
            instructionsText.text = "SELECT ATTACKER";
            _fadingTextCoroutine = _instructionsTextFadingText.FadeTextRoutine();
        }
        else
        {
            _instructionsTextFadingText.StopFadeTextCoroutine(_fadingTextCoroutine);
        }
        instructionsText.enabled = _isPlayerTurn;
    }
    
    #region Fading Coroutines

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fadeInTiles"></param>
    /// <param name="canvas"></param>
    /// <param name="fadeInCanvas"></param>
    /// <returns></returns>
    private IEnumerator FadeAllTiles(bool fadeInTiles, Canvas canvas, bool fadeInCanvas)
    {
        if (fadeInTiles && !_waitedForGameOver)
        {
            _waitedForGameOver = true;
            yield return new WaitForSeconds(pauseBeforeGameOverTime);
        }
        
        _backgroundTilemapRenderer.enabled = true;
        SetOpacityAllTiles(!fadeInTiles);
        
        var bounds = backgroundTilemap.cellBounds;

        for (var x = bounds.xMin; x < bounds.xMax; x++)
        {
            var tilePosition = new Vector3Int(x, bounds.yMax - 1, 0);
            var tile = backgroundTilemap.GetTile(tilePosition);

            if (!tile) continue;

            var tilesToFade = GetDiagonalTiles(tilePosition, true);
            foreach (var tileToFade in tilesToFade)
            {
                StartCoroutine(FadeTile(tileToFade, fadeInTiles));
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
                StartCoroutine(FadeTile(tileToFade, fadeInTiles));
            }
            yield return new WaitForSeconds(tileFadeTime);
        }

        if (!fadeInTiles)
        {
            _backgroundTilemapRenderer.enabled = false;
            _canvas.enabled = true;
        }
        
        _waitedForGameOver = false;
        StartCoroutine(FadeCanvas(fadeInCanvas, canvas, canvas.GetComponent<CanvasGroup>()));
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fadeIn"></param>
    public void FadeDetailsPanel(bool fadeIn)
    {
        StartCoroutine(FadeAllDetailsPanelObjects(fadeIn));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fadeIn"></param>
    /// <returns></returns>
    private IEnumerator FadeAllDetailsPanelObjects(bool fadeIn)
    {
        var originalAlpha = detailsPanel.color.a;
        var targetAlpha = fadeIn ? 1f : 0f;
        
        var elapsedTime = 0f;
        
        while (elapsedTime < canvasFadeTime)
        {
            var alpha = Mathf.Lerp(originalAlpha, targetAlpha, elapsedTime / canvasFadeTime);
            detailsPanel.color = new Color(1, 1, 1, alpha);
            pieceNameText.color = new Color(1, 1, 1, alpha);
            durabilityText.color = new Color(1, 1, 1, alpha);
            foreach (var durabilityCircleImage in durabilityCircleImages)
            {
                if (durabilityCircleImage.gameObject.activeSelf)
                    durabilityCircleImage.color = new Color(1, 1, 1, alpha);
            }
            attackPatternImage.color = new Color(1, 1, 1, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        detailsPanel.color = new Color(1, 1, 1, targetAlpha);
        pieceNameText.color = new Color(1, 1, 1, targetAlpha);
        durabilityText.color = new Color(1, 1, 1, targetAlpha);
        foreach (var durabilityCircleImage in durabilityCircleImages)
        {
            if (durabilityCircleImage.gameObject.activeSelf)
                durabilityCircleImage.color = new Color(1, 1, 1, targetAlpha);
        }
        attackPatternImage.color = new Color(1, 1, 1, targetAlpha);
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
        StartCoroutine(FadeAllTiles(true, levelWonCanvas, true));
    }
    
    /// <summary>
    /// When the player lost the level, enable the level lost canvas and set the input state to canvas enabled.
    /// </summary>
    public void LevelLost()
    {
        StartCoroutine(FadeCanvas(false, _canvas, _canvas.GetComponent<CanvasGroup>()));
        LevelManager.Instance.SetCanvasEnabled(true);
        levelLostCanvas.enabled = true;
        StartCoroutine(FadeAllTiles(true, levelLostCanvas, true));
    }
    
    /// <summary>
    /// When the level was a draw, enable the level draw canvas and set the input state to canvas enabled.
    /// </summary>
    public void LevelDraw()
    {
        StartCoroutine(FadeCanvas(false, _canvas, _canvas.GetComponent<CanvasGroup>()));
        LevelManager.Instance.SetCanvasEnabled(true);
        levelDrawCanvas.enabled = true;
        StartCoroutine(FadeAllTiles(true, levelDrawCanvas, true));
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pieceName"></param>
    /// <returns></returns>
    private string FormatPieceName(string pieceName)
    {
        // get the indices of the capital letters
        var capitalLetterIndices = new List<int>();
        for (var i = 0; i < pieceName.Length; i++)
        {
            if (char.IsUpper(pieceName[i])) capitalLetterIndices.Add(i);
        }
        
        // just get the name of the piece (excluding "Player" or "Opponent")
        pieceName = 
            pieceName.Substring(capitalLetterIndices[1], pieceName.Length - capitalLetterIndices[1]);
        
        // remove the "(Clone)" part of the name, if it's there
        pieceName = pieceName.Contains("(")
            ? pieceName[..pieceName.IndexOf("(", StringComparison.Ordinal)]
            : pieceName;

        return pieceName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedPiece"></param>
    /// <param name="inputState"></param>
    private void UpdateDetailsPanel(ChessPiece selectedPiece, LevelManager.InputState inputState)
    {
        var pieceName = FormatPieceName(selectedPiece.name);
        pieceNameText.text = pieceName.ToUpper();

        var durability = selectedPiece.GetDurability();
        var currentDurability = selectedPiece.GetCurrentDurability();

        for (var i = 0; i < durabilityCircleImages.Length; i++)
        {
            if (i < durability)
            {
                durabilityCircleImages[i].sprite = i < currentDurability ? circleFilledSprite : circleEmptySprite;
                durabilityCircleImages[i].gameObject.SetActive(true);
            }
            else
            {
                durabilityCircleImages[i].gameObject.SetActive(false);
            }
        }

        switch (inputState)
        {
            case LevelManager.InputState.SelectPiece:
                attackPatternImage.gameObject.SetActive(true);
                attackPatternImage.sprite = _pieceNameToAttackPatternSprite[pieceName];
                break;
            case LevelManager.InputState.SelectTarget:
                attackPatternImage.gameObject.SetActive(false);
                break;
            case LevelManager.InputState.Attack:
                break;
            case LevelManager.InputState.CanvasEnabled:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputState), inputState, null);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetPiece"></param>
    /// <param name="isPlayer"></param>
    private void AttackUI(ChessPiece targetPiece, bool isPlayer)
    {
        attackText.enabled = true;
        _attackTextContainerImage.enabled = true;
        var pieceName = FormatPieceName(targetPiece.name);
        attackText.text = $"{pieceName.ToUpper()} TO {Board.Instance.GetBoardTileName(targetPiece.transform.position)}";
        _animator.Play("InGameUIAttack");
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedPiece"></param>
    /// <param name="isPlayer"></param>
    private void UpdateInstructionsText(ChessPiece selectedPiece, bool isPlayer)
    {
        instructionsText.text = "SELECT TARGET";
    }
    
    /// <summary>
    /// Called by the animator when the attack UI animation is done.
    /// </summary>
    public void DisableAttackText()
    {
        attackText.enabled = false;
        _attackTextContainerImage.enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnTurnComplete -= UpdateTurnText;
        GameEvent.OnPlayerTogglePiece -= UpdateDetailsPanel;
        GameEvent.OnConfirmSelectedPiece -= UpdateInstructionsText;
        GameEvent.OnAttackTarget -= AttackUI;
    }
}