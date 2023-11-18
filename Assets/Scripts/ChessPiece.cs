using System;
using System.Collections;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    [SerializeField] private bool isPlayerPiece = true;

    [SerializeField] private SpriteRenderer tileHighlight;
    [SerializeField] private SpriteRenderer progressBar;

    [SerializeField] private Animator progressBarAnimator;

    [SerializeField] private Sprite pieceSelectHighlightSprite;
    [SerializeField] private Sprite pieceAttackHighlightSprite;

    private float _timeToHold;
    private int _numSteps;
    
    private static readonly int ProgressNumber = Animator.StringToHash("ProgressNumber");

    public ExplosionSequence[] ExplosionPattern;

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
    /// <param name="numSteps">The number of steps in the progress bar.</param>
    /// <returns>The progress bar animation coroutine which was started.</returns>
    public Coroutine StartProgressBar(float timeToHold, int numSteps)
    {
        _timeToHold = timeToHold;
        _numSteps = numSteps;
        progressBar.enabled = true;
        return StartCoroutine(ProgressBar(0));
    }
    
    /// <summary>
    /// Advances the progress bar animation step by step.
    /// </summary>
    /// <param name="progressNumber">The number corresponding to the current progress state.</param>
    /// <returns></returns>
    private IEnumerator ProgressBar(int progressNumber)
    {
        progressBarAnimator.SetInteger(ProgressNumber, progressNumber);
        
        yield return new WaitForSeconds(_timeToHold / _numSteps);
        
        progressNumber++;
        if (progressNumber < _numSteps)
        {
            yield return ProgressBar(progressNumber);
        }
        else
        {
            progressBarAnimator.SetInteger(ProgressNumber, 0);
            progressBar.enabled = false;
            
            var inputState = LevelManager.Instance.GetInputState();
            switch (inputState)
            {
                case LevelManager.InputState.SelectPiece:
                    GameEvent.ConfirmSelectedPiece(this);
                    break;
                case LevelManager.InputState.SelectTarget:
                    GameEvent.AttackTarget(this);
                    break;
                case LevelManager.InputState.Attack:
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
        progressBarAnimator.SetInteger(ProgressNumber, 0);
        progressBar.enabled = false;
    }
}