using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    [SerializeField] private float delayBetweenTaps = 0.5f;
    [SerializeField] private float confirmPieceTime = 2f;

    public GameObject PatternOverlayPrefab;
    private GameObject PatternOverlay;
    
    private List<ChessPiece> _pieces = new();
    private ChessPiece _selectedPiece;
    private int _selectedPieceIndex = 0;
    
    private List<ChessPiece> _targetPieces = new();
    private ChessPiece _selectedTargetPiece;
    private int _selectedTargetPieceIndex = 0;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        _pieces = Board.Instance.GetOpponentPieces();
        _selectedPiece = _pieces[_selectedPieceIndex];

        GameEvent.OnConfirmSelectedPiece += ConfirmSelectedPiece;
        GameEvent.OnAttackTarget += Attack;
        GameEvent.OnTurnComplete += OpponentTurn;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OpponentTurn()
    {
        if (LevelManager.Instance.GetIsPlayerTurn()) return;

        _selectedPieceIndex = 0;
        _selectedTargetPieceIndex = 0;
        _selectedPiece.SetHighlight(true, true);
        StartCoroutine(SelectPiece(Random.Range(0, _pieces.Count)));
        // StartCoroutine(SelectPiece(_pieces.Count - 1)); // for deterministic selection
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
            _selectedPiece.StartProgressBar(confirmPieceTime, true);
            yield return null;
        }
        else
        {
            _selectedPiece.SetHighlight(false, true);
            _selectedPieceIndex++;
            _selectedPiece = _pieces[_selectedPieceIndex];
            _selectedPiece.SetHighlight(true, true);
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
            _selectedTargetPiece.SetHighlight(false, false);
            _selectedTargetPieceIndex++;
            _selectedTargetPiece = _targetPieces[_selectedTargetPieceIndex];
            _selectedTargetPiece.SetHighlight(true, false);
            
            // reparent the pattern overlay
            if (PatternOverlay) PatternOverlay.transform.SetParent(_selectedTargetPiece.transform, false);
            
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
        _selectedPiece = selectedPiece;
        _selectedPieceIndex = _pieces.IndexOf(_selectedPiece);
        _selectedPiece.SetHighlight(true, true);
        _targetPieces = Board.Instance.GetPlayerPieces().Append(_selectedPiece).ToList();
        _selectedTargetPiece = _targetPieces[_selectedTargetPieceIndex];
        _selectedTargetPiece.SetHighlight(true, false);

        // setup the pattern
        PatternOverlay = Instantiate(PatternOverlayPrefab, _selectedTargetPiece.transform, false);
        PatternOverlay.transform.Rotate(new Vector3(0, 0, 180));
        PatternRenderer patternRenderer = PatternOverlay.GetComponent<PatternRenderer>();
        patternRenderer.ControllingPiece = _selectedPiece;
        patternRenderer.DrawPattern();
        
        StartCoroutine(SelectTarget(Random.Range(0, _targetPieces.Count)));
        // StartCoroutine(SelectTarget(_targetPieces.Count - 2)); // for deterministic selection
    }

    private void Attack(ChessPiece pieceToAttack, bool isPlayer)
    {
        if (isPlayer) return;
        
        _selectedPiece.SetHighlight(false, true);
        _selectedTargetPiece.SetHighlight(false, false);
        
        var allPieces = Board.Instance.GetAllPieces();
        foreach (var piece in allPieces)
        {
            piece.CheckIfTargeted();
            piece.TakeDamage();
        }
        Destroy(PatternOverlay);
        // todo: spawn attack at pieceToAttack's position
        Debug.Log($"Opponent attacking {pieceToAttack.name} at {pieceToAttack.transform.position}");
        GameEvent.CompleteTurn();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        GameEvent.OnConfirmSelectedPiece -= ConfirmSelectedPiece;
        GameEvent.OnAttackTarget -= Attack;
        GameEvent.OnTurnComplete -= OpponentTurn;
    }
}