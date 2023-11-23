using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    [SerializeField] private bool isPlayerPiece = true;

    [SerializeField] private SpriteRenderer tileHighlight;
    [SerializeField] private SpriteRenderer progressBar;

    [SerializeField] private Sprite pieceSelectHighlightSprite;
    [SerializeField] private Sprite pieceAttackHighlightSprite;
    
    [SerializeField] private int durability = 1;

    private Sprite[] _greenProgressBarSprites;
    private Sprite[] _redProgressBarSprites;
    
    private float _timeToHold;
    private int _numSteps;

    private CameraShake _cameraShake;

    private bool IsTargeted { get; set; } = false;

    public ExplosionSequence[] ExplosionPattern;

    /// <summary>
    /// Initializes the progress bar sprites and the camera shake component.
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void Start()
    {
        (_greenProgressBarSprites, _redProgressBarSprites) = LevelManager.Instance.GetProgressBarSprites();
        if (Camera.main)
        {
            _cameraShake = Camera.main.GetComponent<CameraShake>();
        }
        else
        {
            throw new Exception("Main camera not found!");
        }
    }
    
    /// <summary>
    /// Enables/disables the tile highlight sprite and sets it to the specified color (either green or red).
    /// </summary>
    /// <param name="highlight">True if the highlight is to be enabled, false otherwise.</param>
    /// <param name="green">True if the highlight is to be set to green, false if red.</param>
    public void SetHighlight(bool highlight, bool green)
    {
        tileHighlight.sprite = green ? pieceSelectHighlightSprite : pieceAttackHighlightSprite;
        tileHighlight.enabled = highlight;
    }

    /// <summary>
    /// Returns whether or not this piece is a player piece.
    /// </summary>
    /// <returns>True if this piece is a player piece, false if it is an opponent piece.</returns>
    public bool IsPlayerPiece() => isPlayerPiece;
    
    /// <summary>
    /// Starts the coroutine to animate the progress bar.
    /// </summary>
    /// <param name="timeToHold">The amount of time the player must hold the input key to complete the progress bar.
    /// </param>
    /// <param name="green">True if the progress bar is to be set to green, false if red.</param>
    /// <returns>The progress bar animation coroutine which was started.</returns>
    public Coroutine StartProgressBar(float timeToHold, bool green)
    {
        _timeToHold = timeToHold;
        var progressSprites = green ? _greenProgressBarSprites : _redProgressBarSprites;
        _numSteps = progressSprites.Length;
        progressBar.enabled = true;
        return StartCoroutine(ProgressBar(0, progressSprites));
    }

    /// <summary>
    /// Advances the progress bar animation step by step.
    /// </summary>
    /// <param name="progressNumber">The number corresponding to the current progress state.</param>
    /// <param name="progressSprites"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private IEnumerator ProgressBar(int progressNumber, Sprite[] progressSprites)
    {
        progressBar.sprite = progressSprites[progressNumber];
        
        yield return new WaitForSeconds(_timeToHold / _numSteps);
        
        progressNumber++;
        if (progressNumber < _numSteps)
        {
            yield return ProgressBar(progressNumber, progressSprites);
        }
        else
        {
            progressBar.sprite = progressSprites[0];
            progressBar.enabled = false;
            
            var inputState = LevelManager.Instance.GetInputState();
            switch (inputState)
            {
                case LevelManager.InputState.SelectPiece:
                    GameEvent.ConfirmSelectedPiece(this, LevelManager.Instance.GetIsPlayerTurn());
                    break;
                case LevelManager.InputState.SelectTarget:
                    GameEvent.AttackTarget(this, LevelManager.Instance.GetIsPlayerTurn());
                    break;
                case LevelManager.InputState.Attack:
                    break;
                case LevelManager.InputState.CanvasEnabled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    /// <summary>
    /// Stops the progress bar animation coroutine.
    /// </summary>
    /// <param name="coroutine">The coroutine to stop.</param>
    public void StopProgressBar(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
        progressBar.sprite = _greenProgressBarSprites[0];
        progressBar.enabled = false;
    }

    /// <summary>
    ///  On demand confirmation of targeted status; useful for resolving damage.
    /// </summary>
    public void CheckIfTargeted()
    {
        var col = GetComponent<BoxCollider2D>();
        var filter = new ContactFilter2D().NoFilter();
        var results = new List<Collider2D>();
        col.OverlapCollider(filter, results);
        IsTargeted = false;
        foreach (var result in results.Where(result => result.CompareTag("Projectile")))
        {
            IsTargeted = true;
            break;
        }
    }

    /// <summary>
    /// Plays an explosion on a targeted piece which is attacked and starts the camera shake effect.
    /// </summary>
    public void Explode()
    {
        if (!IsTargeted) return;
        
        // todo: play the animation
        // todo: play the sound
        GameEvent.ShakeCamera();
    }

    /// <summary>
    /// Called by the animator when the explosion animation is complete.
    /// </summary>
    public void TakeDamage()
    {
        durability--;
        if (durability <= 0)
        {
            if (isPlayerPiece)
            {
                Board.Instance.RemoveOpponentPiece(this);
            }
            else
            {
                Board.Instance.RemovePlayerPiece(this);
            }
            
            LevelManager.Instance.CheckForLevelOver();
            
            Destroy(gameObject);
        }
        IsTargeted = false;
    }
}