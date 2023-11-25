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
    
    // heuristic variables
    private (int, int) _bestPieces; // Item1 = piece index, Item2 = target index
    
    // calculated by subtracting damage done to player pieces minus damage done to opponent pieces
    private int _bestNetDamage = -1;

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
        FindBestPieces(_selectedPiece, _selectedPieceIndex);
        StartCoroutine(LoopThroughAllPieces());
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
    /// <returns></returns>
    private IEnumerator LoopThroughAllPieces()
    {
        yield return new WaitForSeconds(delayBetweenTaps);

        if (_selectedPieceIndex == _pieces.Count)
        {
            _selectedPieceIndex = -1;
            yield return SelectPiece(_bestPieces.Item1);
        }
        else
        {
            _selectedPiece.SetHighlight(false, true);
            _selectedPieceIndex++;
            _selectedPiece = _pieces[_selectedPieceIndex];
            _selectedPiece.SetHighlight(true, true);
            FindBestPieces(_selectedPiece, _selectedPieceIndex);
            yield return LoopThroughAllPieces();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void FindBestPieces(ChessPiece currentPiece, int indexOfCurrentPiece)
    {
        var targets = Board.Instance.GetPlayerPieces().ToList().Append(currentPiece).ToList();
        Debug.Log($"targets: {string.Join(", ", targets)}");
        // for (var j = 0; j < targets.Count; j++)
        // {
            // var currentTarget = targets[j];
        var currentTarget = targets[0];
        var patternOverlay = Instantiate(patternOverlayPrefab, currentTarget.transform, false);
        patternOverlay.transform.Rotate(new Vector3(0, 0, 180));
        var patternRenderer = patternOverlay.GetComponent<PatternRenderer>();
        patternRenderer.SetControllingPiece(currentPiece);
        patternRenderer.DrawPattern(true); // todo: change to false
        patternRenderer.transform.SetParent(currentTarget.transform, false);
        var allPieces = Board.Instance.GetAllPieces().ToList();
        var targetedPieces = new List<ChessPiece>();
        foreach (var piece in allPieces)
        {
            var isTargeted = piece.CheckIfTargeted(false);
            Debug.Log($"{piece.name} is targeted: {isTargeted}");
            if (isTargeted) targetedPieces.Add(piece);
        }
        Debug.Log($"targeted pieces: {string.Join(", ", targetedPieces)}");
        // Debug.Log($"targeted pieces: {string.Join(", ", Board.Instance.GetAllPieces().ToList().Where(p => p.CheckIfTargeted(false)))}");
        var breakThing = targets[8];
        var damageDone = GetDamageDone();
        Debug.Log(
            $"using {currentPiece.name} to attack {currentTarget.name} does {damageDone.Item1} damage to player and {damageDone.Item2} damage to opponent");
        var netDamage = damageDone.Item1 - damageDone.Item2;
        Destroy(patternOverlay);
            // if (netDamage <= _bestNetDamage) continue;
            // _bestNetDamage = netDamage;
            // _bestPieces = (indexOfCurrentPiece, j);
        // }
    }

    private (int, int) GetDamageDone()
    {
        var (playerDamage, opponentDamage) = (0, 0);
        var allPieces = Board.Instance.GetAllPieces().ToList();
        foreach (var piece in allPieces.Where(p => p.CheckIfTargeted(false)))
        {
            if (piece.IsPlayerPiece())
            {
                playerDamage += 1;
            }
            else
            {
                opponentDamage += 1;
            }
        }
        
        return (playerDamage, opponentDamage);
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
        patternRenderer.SetControllingPiece(_selectedPiece);
        patternRenderer.DrawPattern(true);
        
        StartCoroutine(SelectTarget(_bestPieces.Item2));
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
            piece.CheckIfTargeted(true);
            piece.Explode();
        }
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