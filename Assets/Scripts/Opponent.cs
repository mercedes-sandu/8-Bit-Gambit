using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    [SerializeField] private float delayBetweenTaps = 0.5f;
    [SerializeField] private float confirmPieceTime = 2f;

    [SerializeField] private GameObject patternOverlayPrefab;
    private GameObject _patternOverlay;
    
    private List<ChessPiece> _pieces = new();
    private ChessPiece _selectedPiece;
    private int _selectedPieceIndex = 0;
    
    private List<ChessPiece> _targetPieces = new();
    private ChessPiece _selectedTargetPiece;
    private int _selectedTargetPieceIndex = 0;

    private Coroutine _flashGreenCoroutine;

    /// <summary>
    /// Sets the opponent's pieces, selected piece, and subscribes to game events.
    /// </summary>
    private void Start()
    {
        _pieces = Board.Instance.GetOpponentPieces().ToList();
        _selectedPiece = _pieces[_selectedPieceIndex];

        GameEvent.OnConfirmSelectedPiece += ConfirmSelectedPiece;
        GameEvent.OnAttackTarget += Attack;
        GameEvent.OnPieceDie += RefreshPieces;
    }

    /// <summary>
    /// 
    /// </summary>
    public void OpponentTurn()
    {
        if (LevelManager.Instance.GetIsPlayerTurn() ||
            LevelManager.Instance.GetInputState() == LevelManager.InputState.CanvasEnabled) return;
        
        _selectedPieceIndex = 0;
        _selectedTargetPieceIndex = 0;
        _selectedPiece = _pieces[_selectedPieceIndex];
        _selectedPiece.SetHighlight(true, true);
        _flashGreenCoroutine = _selectedPiece.StartFlashGreen();
        StartCoroutine(SelectPiece(Random.Range(0, _pieces.Count - 1)));
        // StartCoroutine(SelectPiece(_pieces.Count - 1)); // for deterministic selection
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="isPlayer"></param>
    private void RefreshPieces(ChessPiece piece, bool isPlayer)
    {
        if (isPlayer) return;
        
        _pieces.Clear();
        _pieces = Board.Instance.GetOpponentPieces().ToList();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="indexToSelect"></param>
    /// <returns></returns>
    private IEnumerator SelectPiece(int indexToSelect)
    {
        yield return new WaitForSeconds(delayBetweenTaps);

        if (_selectedPieceIndex == indexToSelect)
        {
            _selectedPiece.StopCoroutine(_flashGreenCoroutine);
            _selectedPiece.StartProgressBar(confirmPieceTime, true);
            yield return null;
        }
        else
        {
            SoundManager.Instance.PlaySelect();
            _selectedPiece.SetHighlight(false, true);
            _selectedPiece.StopFlashGreen(_flashGreenCoroutine);
            _selectedPieceIndex++;
            _selectedPiece = _pieces[_selectedPieceIndex];
            _selectedPiece.SetHighlight(true, true);
            _flashGreenCoroutine = _selectedPiece.StartFlashGreen();
            yield return SelectPiece(indexToSelect);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="indexToSelect"></param>
    /// <returns></returns>
    private IEnumerator SelectTarget(int indexToSelect)
    {
        yield return new WaitForSeconds(delayBetweenTaps);

        if (_selectedTargetPieceIndex == indexToSelect)
        {
            _selectedTargetPiece.StartProgressBar(confirmPieceTime, false);
            yield return null;
        }
        else
        {
            SoundManager.Instance.PlaySelect();
            _selectedTargetPiece.SetHighlight(false, false);
            _selectedTargetPieceIndex++;
            _selectedTargetPiece = _targetPieces[_selectedTargetPieceIndex];
            _selectedTargetPiece.SetHighlight(true, false);
            
            // reparent the pattern overlay
            if (_patternOverlay) _patternOverlay.transform.SetParent(_selectedTargetPiece.transform, false);
            
            yield return SelectTarget(indexToSelect);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectedPiece"></param>
    /// <param name="isPlayer"></param>
    private void ConfirmSelectedPiece(ChessPiece selectedPiece, bool isPlayer)
    {
        if (isPlayer) return;
        _selectedPiece.SetHighlight(false, true);
        _selectedPiece.StopCoroutine(_flashGreenCoroutine);
        _selectedPiece = selectedPiece;
        _selectedPieceIndex = _pieces.IndexOf(_selectedPiece);
        _selectedPiece.SetHighlight(true, true);
        _targetPieces = Board.Instance.GetPlayerPieces().ToList().Append(_selectedPiece).ToList();
        _selectedTargetPiece = _targetPieces[_selectedTargetPieceIndex];
        _selectedTargetPiece.SetHighlight(true, false);

        // setup the pattern
        _patternOverlay = Instantiate(patternOverlayPrefab, _selectedTargetPiece.transform, false);
        _patternOverlay.transform.Rotate(new Vector3(0, 0, 180));
        var patternRenderer = _patternOverlay.GetComponent<PatternRenderer>();
        SoundManager.Instance.StopHoldAudio();
        patternRenderer.SetControllingPiece(_selectedPiece);
        patternRenderer.DrawPattern(true);
        
        StartCoroutine(SelectTarget(Random.Range(0, _targetPieces.Count)));
        // StartCoroutine(SelectTarget(_targetPieces.Count - 2)); // for deterministic selection
    }

    /// <summary>
    /// Launches the opponent's selected piece's attack at the specified target piece to attack.
    /// </summary>
    /// <param name="pieceToAttack">The piece to attack.</param>
    /// <param name="isPlayer">True if it is the player's turn, false otherwise.</param>
    private void Attack(ChessPiece pieceToAttack, bool isPlayer)
    {
        if (isPlayer) return;
        
        _selectedPiece.SetHighlight(false, true);
        _selectedTargetPiece.SetHighlight(false, false);
        
        _patternOverlay.GetComponent<PatternRenderer>().TriggerAnimations();

        var allPieces = Board.Instance.GetAllPieces().ToList();
        foreach (var piece in allPieces)
        {
            piece.CheckIfTargeted();
            piece.Explode();
        }
        SoundManager.Instance.StopHoldAudio();
        SoundManager.Instance.PlayExplosion();
        GameEvent.ShakeCamera();
        GameEvent.CompleteTurn();
    }

    /// <summary>
    /// Unsubscribes from game events.
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnConfirmSelectedPiece -= ConfirmSelectedPiece;
        GameEvent.OnAttackTarget -= Attack;
        GameEvent.OnPieceDie -= RefreshPieces;
    }
}